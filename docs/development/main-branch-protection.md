\# Protección de main



El ruleset Protect main protege únicamente la rama main.

Está activo desde el 26 de octubre de 2026.



\## Reglas



\- Los cambios requieren un pull request.

\- No se requieren aprobaciones de otra persona.

\- Las conversaciones de revisión deben estar resueltas.

\- El check Build and Tests de GitHub Actions debe pasar.

\- La rama del pull request debe estar actualizada con main.

\- Se bloquean los force pushes y la eliminación de main.

\- La lista de excepciones está vacía.



\## Flujo de trabajo



1\. Actualizar main y crear una rama de trabajo.

2\. Realizar los cambios y subir la rama.

3\. Abrir un pull request hacia main.

4\. Esperar el resultado de Build and Tests.

5\. Resolver las conversaciones de revisión.

6\. Si main cambió, actualizar la rama y esperar las pruebas.

7\. Integrar el pull request cuando cumpla las reglas.



\## Administración



La configuración se encuentra en GitHub, dentro de

Settings > Rules > Rulesets > Protect main.



Este documento describe la configuración.

Las reglas se aplican desde GitHub.



\## Validación pendiente



Validar mediante el pull request de esta documentación

que Build and Tests sea obligatorio y que el merge

quede habilitado cuando se cumplan las reglas.

