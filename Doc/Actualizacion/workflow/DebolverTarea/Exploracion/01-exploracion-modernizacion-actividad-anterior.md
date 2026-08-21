# Exploración técnica — Modernización de devolver a actividad anterior

## Decisión inicial

Es viable modernizar **Devolver → Elegir actividad anterior** como una operación moderna independiente. No debe reutilizar el recorrido de **Continuar flujo** ni el postback Web Forms actual.

El alcance de esta exploración cubre exclusivamente la elección de una actividad anterior. **Usuario anterior** usa un recorrido distinto y queda fuera de este primer corte.

## Comportamiento actual

La acción está en `workflow/Webworkflow.aspx`, dentro del menú **Devolver**. Actualmente usa controles ocultos, `UpdatePanel`, `ModalPopupExtender`, campos `Hidden_*` y confirmación JavaScript.

La resolución del destino tiene dos recorridos:

| Contexto | Cómo identifica la actividad anterior | Ejecución actual |
| --- | --- | --- |
| Flujo | Busca conectores cuyo destino es la actividad actual del flujo. | `Enviar_actividad_por_conector_flujo_de_trabajo_anterior`. |
| Ruta | Busca actividades cuya siguiente actividad es la actividad actual. | Handler tradicional `Button_activa_enviar_actividad_ruta_Click`. |

La entrada común es `ClassWorkflow.Activa_devolver_actividades_anteriores`. Para flujo usa `Solicita_lista_actividade_anteriores_flujo_trabajo`; para ruta usa `Solicita_Listado_actividades_anteriores_ruta`.

Por tanto, la devolución no es un envío normal hacia un conector saliente: es una transición por un conector **entrante** al origen actual.

## Límite de la arquitectura moderna actual

`ServicioTransicionTarea`, `PreviewEnviarTarea` y `EjecutarEnvioTarea` solo resuelven conectores salientes desde la actividad actual. No pueden utilizarse sin extensión para una devolución.

El adaptador moderno existente a `ClassWorkflow.Terminar_Tarea_Workflow` sí puede servir de base, siempre que el destino se reconstruya en servidor a partir del conector entrante y no del valor publicado por el navegador.

## Diseño propuesto

Crear una capacidad separada, con contratos y servicios propios:

1. `PreviewDevolverActividad(idTarea)`
   - Solo lectura (`SELECT`).
   - Valida contexto autenticado, tarea activa y permiso de devolución.
   - Resuelve únicamente conectores entrantes que correspondan a la tarea, ruta o flujo actuales.
   - Devuelve actividad, usuario o grupo destino mínimo, tipo de contexto, token de versión y bloqueos funcionales.

2. `EjecutarDevolverActividad(idTarea, idConector, tokenVersion)`
   - Adquiere el lock de concurrencia.
   - Relee tarea, token, contexto, permiso y conector entrante dentro del lock.
   - Invoca un adaptador específico de devolución hacia `Terminar_Tarea_Workflow`.
   - Normaliza resultado, auditoría y advertencias sin exponer SQL, sesión ni excepciones.

3. Presentación moderna
   - Reutiliza el patrón de modal accesible, confirmación, foco, Escape, bloqueo durante envío y restauración de bandeja.
   - Muestra la acción como **Devolver a actividad anterior**.
   - No usa postbacks ocultos, `GridView`, `ModalPopupExtender` ni campos `Hidden_*` para autorizar o ejecutar.

## Regla obligatoria: no tratar respuestas

La capacidad moderna de devolución no debe tratar respuestas en ninguna capa:

- No consulta estado de respuesta, radicado o confirmación.
- No bloquea por condiciones de respuesta.
- No reasigna, crea, actualiza ni audita respuestas.
- No llama a `Classgestionrespuesta`, `Verifica_respuesta_*` ni `Reasigna_respuesta_envia_tarea_usuario`.
- No agrega datos ni reglas de respuesta a los DTO, endpoints, mensajes o pruebas.

El recorrido Web Forms actual contiene lógica de validación y reasignación de respuestas; no se puede reutilizar como ejecutor de la versión moderna.

## Riesgos que debe corregir la modernización

- El menú legado se muestra al seleccionar una tarea y la comprobación de permiso no queda revalidada de forma uniforme en todos los caminos de ejecución.
- El identificador de conector se conserva en el cliente; la implementación moderna debe tratarlo solo como una referencia no confiable y resolverlo de nuevo en servidor.
- Ruta y flujo tienen consultas y handlers diferentes; el contrato público puede ser único, pero los repositorios deben preservar la validación específica de cada contexto.
- No existe cobertura automatizada ni una especificación OpenSpec vigente para esta operación.

## Pruebas requeridas

- Preview sin escritura y sin llamadas a componentes de respuesta.
- Permiso de devolución ausente, tarea inexistente o cerrada y contexto ruta/flujo inconsistente.
- Conector entrante inexistente, ajeno al contexto, retirado o manipulado desde el navegador.
- Token vencido, doble solicitud y lock ocupado.
- Éxito, error reintentable, advertencia y auditoría sanitizada.
- Modal bloqueado durante ejecución, cancelación, teclado, foco, Escape, responsive y restauración correcta de la bandeja.
- Prueba estática y de unidad que garantice la ausencia de referencias a `Classgestionrespuesta`, `Verifica_respuesta_*` y `Reasigna_respuesta_envia_tarea_usuario`.

No se ejecutará E2E autenticada, carga ni una tarea real sin autorización explícita del ambiente y de las cuentas de prueba.

## Conclusión

La modernización es factible y recomendable. Debe implementarse como **devolución por conector entrante**, con preview y ejecución propios, y mantener fuera de alcance la devolución a usuario anterior y cualquier tratamiento de respuestas.
