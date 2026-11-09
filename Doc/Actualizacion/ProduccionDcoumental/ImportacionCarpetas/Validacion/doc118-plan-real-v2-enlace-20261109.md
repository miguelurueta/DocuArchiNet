# DOC-118 — estado del plan real tras enlace/driver

Este corte complementa el [plan RV-08](doc118-plan-real-v2-20261109.md), conservado con su base y límites. No reemplaza sus casos Q1..Q4, nombres, recursos, secuencia, cortes o topes propuestos. No concede autorización de despliegue, login o ejecución; tampoco afirma disponibilidad del ambiente/cuentas.

| Dependencia | Situación del candidato local |
| --- | --- |
| P1:composición V2 para intentos nuevos | IMPLEMENTADA_Y_PROBADA_LOCALMENTE en6.9. Revisión de servidor, misma persistencia, autoridad y sin fallback/backfill; método compartido intacto. Falta despliegue/calificación autorizados. |
| P2:driver compartido de recuperación | IMPLEMENTADO_Y_PROBADO_LOCALMENTE en6.10. Solicitud/respuesta cerradas, mismo transporte/CSRF, readOnly y cero retries. No constituye por sí solo un runner de Q1..Q4. |
| P3:pérdida de acuse tras commit | SIN_IMPLEMENTAR en el enlace de prueba del runtime; requiere alcance local explícito, pruebas y revisión. Los fallos simulados del harness no son un hook instalado. |
| P4:sitio/proveedor/MySQL/IIS/observador | SIN_CALIFICAR. Continúan las restricciones de época, proveedor y llamada real; no inferir capacidad de los procesos sintéticos. |
| P5:candidato/runner y autorización | NO_AUTORIZADO para ejecución real. Fijar SHA del paquete y runner después de resolver P3; preparar la autorización concreta de despliegue/calificación/recursos/topes como paso final. |

Se mantienen como propuesta2 lotes/4 raíces/4 PDFs,2 sesiones,4 expedientes,2 invocaciones documentales máximas,1000 HTTP totales/100 de negocio y180 SELECT, con reservas y cortes del plan original. No son capacidad disponible ni aumentan el consumo histórico1276 HTTP/113 SELECT/9 aperturas. No usar recursos históricos18/21/24 ni41/53/69. Los casos reales originales COORD-02/03 siguen requiriendo su propia evidencia.

Resultado: **P1/P2 resueltos en código candidato; plan real todavía NO_EJECUTABLE/NO_AUTORIZADO por P3/P4/P5.** Próximo trabajo local:preparar el enlace acotado de pérdida de acuse y su conductor mediante los puertos existentes, sin modificar almacenamiento compartido o agregar un endpoint de fallos público.
