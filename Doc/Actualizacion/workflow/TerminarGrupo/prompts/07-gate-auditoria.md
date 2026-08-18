# 07 — Gate existente, auditoría y rollback

## ROL ESPERADO

Actúa como arquitecto de release y calidad para Workflow legacy, controles de habilitación fail-closed y despliegues reversibles.

## OBJETIVO

Integrar `Enviar a grupo` en el gobierno, auditoría y rollback de la modernización existente sin crear una segunda configuración ni cambiar el estado operativo del gate.

## RESTRICCIONES CRITICAS

- Leer y aplicar `prompts/00-contexto-obligatorio.md`.
- Reutilizar exclusivamente `IWorkflowModernFeatureGate`, `WorkflowCentroTrabajoModernActive` y el bootstrap de Presentation ya existentes.
- No crear opt-in, bandera, appSetting, fuente de configuración, ASMX o evaluación paralela para grupo.
- No activar, desactivar ni editar configuración durante esta etapa; cualquier operación futura autorizada debe finalizar con `WorkflowCentroTrabajoModernActive=false` y usuarios/grupos vacíos.
- No guardar en auditoría SQL, Session, token, payloads, documentos, credenciales ni datos personales innecesarios.
- No revertir transiciones confirmadas con SQL, JavaScript o llamadas directas a `Cambia_Estado`.

## REQUISITOS POSITIVOS

1. Asegurar que preview, ejecución y bootstrap de grupo consuman y revaliden el gate existente de forma fail-closed.
2. Registrar auditoría sanitizada con `Canal=MODERNO` y `Mecanismo=ASMX_ENVIO_GRUPO`, incluyendo tarea, ruta, origen, actividad destino, resultado, código y duración; conector en cero cuando aplique.
3. Diferenciar auditoría de grupo de la transición por conector sin cambiar el contrato de esta última.
4. Documentar rollback: gate existente inactivo implica que nuevos intentos usan postback legacy, sin migración ni reversión de datos.

## SECUENCIA FUNCIONAL

1. El bootstrap de página consulta la evaluación existente del gate y solo enlaza la experiencia moderna de grupo si está activa.
2. `PreviewEnviarGrupo` y `EjecutarEnvioGrupo` reevalúan el mismo gate en servidor antes de resolver o ejecutar.
3. Si está inactivo, el cliente conserva el postback legacy y las llamadas ASMX de grupo devuelven bloqueo funcional sin fallback automático.
4. Si ocurre rollback autorizado, las transiciones ya confirmadas permanecen; los nuevos intentos vuelven al camino Web Forms sin migrar ni modificar estados.

## REGLAS DE ANTIRREGRESION

- Mantener intacta la única evaluación de gate existente y el comportamiento de continuar flujo.
- No cambiar configuración, precedencia, payload, códigos, bootstrap ni pruebas de `WorkflowCentroTrabajoModernActive` para continuar flujo.
- No modificar los endpoints de continuar ni registrar assets modernos de grupo cuando el gate esté inactivo.
- Ejecutar las pruebas existentes del gate, rollback y presentación de continuar junto con las nuevas; detener la etapa ante cualquier diferencia.

## CRITERIOS DE ACEPTACION

- Existe una sola fuente de habilitación moderna para la página y ambos endpoints.
- Configuración ausente o inválida bloquea los endpoints modernos de grupo sin fallback ASMX automático.
- El rollback no necesita migración ni altera transiciones ya confirmadas.
- La auditoría es útil para soporte y no expone información sensible.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas de gate activo/inactivo/inválido, exclusión, bloqueo de llamada directa, serialización sanitizada de auditoría y rollback visual al postback legacy. Ejecutar `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug` y las pruebas focales de gate/auditoría configuradas en el repositorio; registrar comandos, códigos de salida y resultados. Si no están disponibles, documentar limitación/QA local. No activar gates ni ejecutar E2E/carga sin autorización.

## DOCUMENTACION TECNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarGrupo/07-gate-auditoria/` con fuente única de gate, alcance, auditoría, código de rollback, responsable operativo, matriz de pruebas y diagrama de decisión.

## ENTREGABLE FINAL

Entregar cambios de integración de gate/auditoría, pruebas y compilación, documentación de rollback y confirmación explícita de que no se creó ni activó una segunda configuración.
