# Revisión estructural de los prompts de importación

Fecha: 2026-10-19. Alcance: actualización documental de los prompts 02–13. No se implementaron componentes, no se creó SQL ni se ejecutaron pruebas autenticadas o cambios de base de datos.

## Comando y evidencia

Por cada archivo se ejecutó desde la raíz del repositorio:

```powershell
npm.cmd --prefix Tools/opsxj run opsxj:prompt-review -- <ruta-absoluta-del-prompt> --tech-profile legacy-webforms-vb --no-fix
```

Se eligió el perfil explícito del repositorio. No se aplicó la corrección automática ni se modificó el validador. El comando sobrescribe `.opsxj/reports/prompt-review-report.json` en cada ejecución; por eso se conservaron resultados individuales. La evidencia consolidada con huellas SHA-256 de los prompts está en [08-evidencia-revision-prompts.json](08-evidencia-revision-prompts.json).

| Prompt | Antes: bloqueantes / mayores | Después | Bloqueantes / mayores / menores |
|---|---|---|---|
| 02 Perfil | 9 / 5 | PASS | 0 / 0 / 0 |
| 03 Carga general | 4 / 4 | PASS | 0 / 0 / 0 |
| 04 Diseño | 8 / 8 | PASS | 0 / 0 / 0 |
| 05 Ventana | 4 / 7 | PASS | 0 / 0 / 0 |
| 06 Plan | 8 / 6 | PASS | 0 / 0 / 0 |
| 07 Lotes y recepción | 6 / 6 | PASS | 0 / 0 / 0 |
| 08 Expedientes | 8 / 6 | PASS | 0 / 0 / 0 |
| 09 Almacenamiento | 6 / 7 | PASS | 0 / 0 / 0 |
| 10 Coordinación | 8 / 8 | PASS | 0 / 0 / 0 |
| 11 Seguimiento | 7 / 8 | PASS | 0 / 0 / 0 |
| 12 Verificación | 5 / 8 | PASS | 0 / 0 / 0 |
| 13 Ejecutor opcional | 6 / 9 | PASS | 0 / 0 / 0 |

Cada resultado mantiene una observación informativa `MANUAL_REVIEW_RECOMMENDED`: el validador no sustituye una revisión humana de coherencia semántica.

## Correcciones y comprobación de coherencia

| Aspecto | Tratamiento |
|---|---|
| Estructura del prompt | Rol, objetivo, restricciones, aceptación, pruebas, documentación y entrega explícitos; se conservaron los requisitos detallados existentes. |
| Rutas multicapa | Ubicaciones comunes propuestas, propietarios por entrega, inventario previo y registro en el vbproj; sin afirmar que existan archivos nuevos. |
| Transporte | Controller describe una responsabilidad de ASMX/ASHX; no se introduce MVC. |
| Alcance documental 04 | E2E y MSBuild no aplican al diseño; se exige revisión manual de contratos y transiciones. |
| Dependencias de pruebas | 07-A prueba persistencia sin esperar 06; 07-B prueba recepción. 08/09 no crean endpoints públicos para probar adaptadores. 12 verifica transversalmente lo integrado. |
| Pruebas reales | Infraestructura existente, autorización de ambiente/cuentas/datos descartables, secretos efímeros y evidencia saneada. Sin autorización se registra el impedimento, no evidencia ficticia. |
| Integridad | Se conservan EXPEDIENTE y LOTE en modo completo, NO_APLICA en parcial y snapshot inmutable. No se agrega rollback global. |
| No regresión | SII intacto, carga individual preservada, ausencia de cambios al handler compartido y rechazo de alteraciones de pruebas para ocultar regresiones. |
| SQL y ejecución opcional | Scripts manuales siguen perteneciendo a 02/07; 13 exige activar su alcance y decidir host. |

PASS acredita únicamente cumplimiento estructural del texto revisado. No certifica implementación, compilación, compatibilidad desplegada ni ausencia de regresiones. Los pendientes del diseño y las autorizaciones operativas conservan vigencia. Un cambio posterior del prompt requiere repetir la revisión y actualizar su evidencia.
