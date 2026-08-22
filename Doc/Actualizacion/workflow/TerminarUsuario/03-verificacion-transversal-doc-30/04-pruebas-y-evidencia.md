# Pruebas, evidencia y dictamen

- Ticket: DOC-30
- Cambio OpenSpec: doc-30-verificacion-transversal-enviar-usuario
- Clasificación: cross_cutting

## Matriz de verificación

| Área | Evidencia | Resultado |
| --- | --- | --- |
| Contrato directo | Inspección de ASMX, DTOs, servicios, adaptadores y pruebas de contrato. | Aprobado: usuario–actividad–token; sin `IdConector`. |
| Seguridad y concurrencia | Validadores, `GET_LOCK`, política de respuesta y auditoría. | Aprobado: revalidación bajo lock; sin reasignación. |
| Interfaz y accesibilidad | CJS y QA visual no autenticada. | Aprobado: búsqueda, cursor, respuestas obsoletas, foco, Escape, responsive y bloqueo. |
| Compatibilidad | Pruebas de Grupo, transición, gate y presentación. | Aprobado: Continuar flujo conserva `IdConector` y no recibe estado de usuario. |
| Compilación | `msbuild .\GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /verbosity:minimal /clp:ErrorsOnly` | Correcta, sin errores. |
| Pruebas locales | `node --test` sobre las nueve suites focales de usuario, grupo, transición, confirmación, presentación y gate. | 66 correctas, 0 fallos. |

## QA visual no autenticada

El recorrido revisado cubre apertura del modal, búsqueda, transición visual de resultados, selección, recarga y cierre. El modal conserva su geometría y el disparador de usuario permanece en el grupo de transferencias. Las comprobaciones de teclado, foco, Escape, bloqueo y representación responsiva están además cubiertas por CJS.

## Limitaciones y riesgos

No se repitieron E2E autenticados, carga, concurrencia mutante, activación de gate, consultas de ambiente ni despliegue. Esas operaciones no son requisito de esta compuerta y requieren autorización específica. No se encontró un escenario crítico fallido ni se creó ticket correctivo.

## Dictamen técnico

**Apto para solicitar aprobación operativa.** La decisión no despliega la versión ni autoriza un ambiente; la operación deberá registrar matriz de ambiente, responsables, ventana, validaciones `SELECT` y procedimiento de reversión aprobado.
