# Inventario de superficies verificadas

- Ticket: DOC-30
- Cambio OpenSpec: doc-30-verificacion-transversal-enviar-usuario
- Clasificación: cross_cutting

## Componentes verificados

| Superficie | Responsabilidad comprobada |
| --- | --- |
| `WebServiceWorkflowModern.asmx.vb` | Transporte ASMX directo, sin motor legacy ni `IdConector` en usuario. |
| `ServicioEnvioUsuarioTarea` y validadores | Revalidación, requisitos, lock y auditoría del envío. |
| Adaptadores y repositorio de usuario | Destino autorizado, frontera legacy directa y sanitización. |
| `Webworkflow.aspx` y code-behind | Disparador moderno sin fallback de usuario. |
| `workflow-user-send-ui.js` | Búsqueda, cursor, obsolescencia y selección. |
| `workflow-user-send-confirmation.js` | Confirmación, envío y guardia de cierre. |
| `workflow-transition-ui.js` | Compatibilidad independiente de Continuar flujo por conector. |

## Componentes reutilizados

`ConfirmationDialog`, la presentación correlacionada y los contratos de transición existentes se conservan con límites explícitos. DOC-30 no creó componentes nuevos ni modificó los existentes.
