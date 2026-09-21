#!/usr/bin/env python3
"""Import ServiceDesk_MVP_Backlog.csv into GitHub Issues + GitHub Projects.

Default mode is DRY RUN. Use --apply to make changes.
Requires: Python 3, GitHub CLI (gh), and `gh auth refresh -s project`.
"""
from __future__ import annotations
import argparse, csv, json, subprocess, sys
from pathlib import Path

PROJECT_FIELDS = ("Status", "Priority", "Work Type", "Area")

class GhError(RuntimeError):
    pass

def run_gh(args, check=True):
    cmd = ["gh", *args]
    try:
        p = subprocess.run(cmd, text=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    except FileNotFoundError as e:
        raise GhError("GitHub CLI (gh) was not found.") from e
    if check and p.returncode != 0:
        raise GhError(f"Command failed:\n  {' '.join(cmd)}\n\n{p.stderr.strip() or p.stdout.strip()}")
    return p

def gh_json(args):
    p = run_gh(args)
    try:
        return json.loads(p.stdout)
    except json.JSONDecodeError as e:
        raise GhError(f"Could not parse JSON from: gh {' '.join(args)}") from e

def unpack(payload, key):
    if isinstance(payload, list):
        return payload
    if isinstance(payload, dict) and isinstance(payload.get(key), list):
        return payload[key]
    return []

def detect_repo():
    data = gh_json(["repo", "view", "--json", "nameWithOwner"])
    repo = data.get("nameWithOwner")
    if not repo:
        raise GhError("Could not detect repository. Run inside the repo or pass --repo OWNER/REPO.")
    return repo

def find_project(owner, title):
    data = gh_json(["project", "list", "--owner", owner, "--limit", "100", "--format", "json"])
    matches = [p for p in unpack(data, "projects") if p.get("title") == title]
    if not matches:
        raise GhError(f"Project '{title}' not found for owner '{owner}'.")
    if len(matches) > 1:
        raise GhError(f"More than one Project named '{title}' exists for '{owner}'.")
    return matches[0]

def read_csv(path):
    required = {"Order","Phase","Title","Work Type","Area","Status","Priority"}
    if not path.exists():
        raise GhError(f"CSV file not found: {path}")
    with path.open("r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f)
        missing = required - set(reader.fieldnames or [])
        if missing:
            raise GhError("CSV missing columns: " + ", ".join(sorted(missing)))
        rows = []
        for row in reader:
            clean = {k:(v or "").strip() for k,v in row.items()}
            if clean["Title"]:
                rows.append(clean)
    rows.sort(key=lambda r: int(r["Order"]))
    return rows

def project_fields(project_number, owner):
    data = gh_json(["project","field-list",str(project_number),"--owner",owner,"--limit","100","--format","json"])
    return unpack(data, "fields")

def option_names(field):
    return {str(o.get("name")) for o in (field.get("options") or []) if isinstance(o, dict) and o.get("name")}

def validate_fields(rows, fields):
    by_name = {str(f.get("name")): f for f in fields if f.get("name")}
    errors = []
    for name in PROJECT_FIELDS:
        field = by_name.get(name)
        if not field:
            errors.append(f"Missing Project field: {name}")
            continue
        required = {r[name] for r in rows if r[name]}
        available = option_names(field)
        missing = required - available
        if missing:
            errors.append(f"Field '{name}' missing option(s): {', '.join(sorted(missing))}")
    if errors:
        raise GhError("Project validation failed. Nothing created.\n- " + "\n- ".join(errors))

def existing_issues(repo):
    data = gh_json(["issue","list","-R",repo,"--state","all","--limit","1000","--json","title,url,number,state"])
    result = {}
    for issue in data:
        result.setdefault(issue["title"], []).append(issue)
    return result

def body(row):
    return f"""## Backlog metadata\n\n- **Order:** {row['Order']}\n- **Phase:** {row['Phase']}\n- **Work Type:** {row['Work Type']}\n- **Area:** {row['Area']}\n- **Priority:** {row['Priority']}\n\n## Description\n\nImported from the ServiceDesk MVP master backlog.\n\nDetailed scope and acceptance criteria will be added during backlog refinement before development starts.\n"""

def create_issue(repo, row):
    p = run_gh(["issue","create","-R",repo,"--title",row["Title"],"--body",body(row)])
    urls = [x.strip() for x in p.stdout.splitlines() if x.strip().startswith("https://github.com/")]
    if not urls:
        raise GhError(f"Created issue '{row['Title']}' but could not read its URL.")
    return urls[-1]

def add_to_project(number, owner, url):
    p = run_gh(["project","item-add",str(number),"--owner",owner,"--url",url,"--format","json"], check=False)
    if p.returncode == 0:
        return
    text = (p.stderr + "\n" + p.stdout).lower()
    if any(s in text for s in ("already exists", "already in", "already added", "content already exists")):
        return
    raise GhError(p.stderr.strip() or p.stdout.strip())

def set_field(number, owner, url, field, value):
    run_gh(["project","item-edit",str(number),"--owner",owner,"--url",url,"--field",field,"--value",value])

def main():
    ap = argparse.ArgumentParser(description="Import ServiceDesk backlog into GitHub Issues + Project.")
    ap.add_argument("--csv", default="ServiceDesk_MVP_Backlog_v2.csv")
    ap.add_argument("--repo", help="OWNER/REPO; default detects current repo")
    ap.add_argument("--project", default="@carlosprietobarron's ServiceDesk MVP")
    ap.add_argument("--project-owner", help="Defaults to repository owner")
    ap.add_argument("--apply", action="store_true", help="Make changes; without this it is dry-run")
    args = ap.parse_args()

    try:
        run_gh(["auth","status"])
        repo = args.repo or detect_repo()
        owner = args.project_owner or repo.split("/",1)[0]
        rows = read_csv(Path(args.csv))
        project = find_project(owner, args.project)
        number = int(project["number"])
        validate_fields(rows, project_fields(number, owner))
        existing = existing_issues(repo)

        duplicates = [t for t, matches in existing.items() if len(matches) > 1 and any(r["Title"] == t for r in rows)]
        if duplicates:
            raise GhError("Duplicate exact issue titles already exist:\n- " + "\n- ".join(sorted(duplicates)))

        print(f"Repository: {repo}")
        print(f"Project:    {args.project} (#{number}, owner {owner})")
        print(f"CSV:        {args.csv}")
        print(f"Mode:       {'APPLY' if args.apply else 'DRY RUN'}\n")

        create_n = reuse_n = 0
        for r in rows:
            if existing.get(r["Title"]):
                reuse_n += 1
                print(f"[REUSE]  {r['Order']:>2}  {r['Title']}")
            else:
                create_n += 1
                print(f"[CREATE] {r['Order']:>2}  {r['Title']}")
        print(f"\nWould create: {create_n}; reuse: {reuse_n}; total: {len(rows)}")

        if not args.apply:
            print("\nDRY RUN complete. Nothing changed. Run again with --apply if correct.")
            return 0

        print("\nApplying import...\n")
        created = reused = 0
        for r in rows:
            matches = existing.get(r["Title"], [])
            if matches:
                url = matches[0]["url"]
                reused += 1
                tag = "REUSE"
            else:
                url = create_issue(repo, r)
                existing[r["Title"]] = [{"title": r["Title"], "url": url}]
                created += 1
                tag = "CREATE"
            print(f"[{tag}] {r['Order']:>2}  {r['Title']}")
            add_to_project(number, owner, url)
            for field in PROJECT_FIELDS:
                if r[field]:
                    set_field(number, owner, url, field, r[field])

        print(f"\nDone. Created {created}, reused {reused}, processed {len(rows)}.")
        print(f"Open Project: gh project view {number} --owner {owner} --web")
        return 0

    except GhError as e:
        print(f"\nERROR\n{e}", file=sys.stderr)
        print("\nIf this is a Projects permission error, run:\n  gh auth refresh -s project", file=sys.stderr)
        return 1
    except KeyboardInterrupt:
        print("\nCanceled.", file=sys.stderr)
        return 130

if __name__ == "__main__":
    raise SystemExit(main())
