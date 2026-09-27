# DOC-83 — Resumen técnico

- Ticket: DOC-83
- Cambio OpenSpec: `doc-83-pruebas-servicio-sii-enlace`
- Clasificación: transversal

DOC-83 consolida la verificación de la importación SII para actividades ENLASE. No cambia lógica productiva: convierte la relación entre nueve riesgos altos/muy altos, pruebas determinísticas, escenarios E2E DOC-80/81/82, controles y evidencia en un manifiesto validable.

El cambio agrega un validador, cuatro pruebas de su contrato y un comando de regresión de 145 pruebas. Preserva el gate global, la asignación explícita en `Buttonaceptar_Click`, la ruta legacy y la separación entre recursos de importación y asignación.

Reversa: retirar manifiesto, validador, pruebas, lanzador y documentación DOC-83. No existe migración ni estado productivo que revertir.
