# Arquitectura de verificación

- Ticket: DOC-30
- Cambio OpenSpec: doc-30-verificacion-transversal-enviar-usuario
- Clasificación: cross_cutting

## Arquitectura de la solución

DOC-30 observa el snapshot integrado sin alterarlo. Correlaciona cuatro fuentes: inspección estática de los límites Web Forms/ASMX/VB.NET/JavaScript, batería CJS focal, compilación MSBuild y QA visual no autenticada. El resultado es una matriz de evidencia sanitizada y un dictamen técnico único.

## Alcance y compatibilidad

La verificación cubre el envío directo por usuario–actividad–token, preview de solo lectura, validación bajo lock, auditoría y la interfaz moderna. Mantiene como contratos externos los endpoints de Grupo y Continuar flujo con `IdConector`; no crea otra ruta UI, no cambia esquema y no modifica configuración.

## Decisión arquitectónica

La evidencia técnica habilita únicamente solicitar la aprobación operativa. Cualquier ejecución autenticada, carga, activación o despliegue requiere una decisión independiente del ambiente.
