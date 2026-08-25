# Flujo, seguridad y límites

- Ticket: DOC-34
- Cambio OpenSpec: `doc-34-verificacion-transversal-devolver-tarea`

## Preview

1. ASMX obtiene contexto autenticado y crea el servicio de devolución.
2. El servicio valida intención, contexto, permiso, tarea activa y cursor ligado al snapshot.
3. El repositorio ejecuta lecturas parametrizadas de Ruta o Flujo sobre conectores entrantes autorizados.
4. El servicio publica destinos mínimos, token y cursor protegido; no adquiere lock, no invoca el motor y no audita.

## Ejecución

1. ASMX recibe solo tarea, conector y token.
2. `ServicioDevolverActividad` adquiere exclusión por tarea y, dentro del lease, relee tarea, permiso, token y conector entrante.
3. El adaptador específico ejecuta la transición aprobada con `Page = Nothing`; no construye ni invoca métodos de respuestas.
4. La auditoría conserva un código funcional y una referencia saneada; una falla posterior agrega advertencia sin revertir un éxito.

## Interfaz y accesibilidad

La UI obtiene un preview, permite seleccionar una actividad y abre la confirmación con la misma terna mínima. Mientras espera una respuesta, la confirmación compartida bloquea doble envío, cancelar, cierre, Escape y abandono; el marcado usa diálogo modal, foco y regiones `aria-live`. Las políticas CJS verifican que los scripts de devolución no consultan `WorkflowCentroTrabajoModernActive` y que la actividad anterior no tiene postback, handler ni fallback Web Forms.

## Límites operativos

DOC-34 no ejecuta operaciones autenticadas, carga, consultas de ambiente ni despliegue. Una futura QA visual no autenticada debe ser autorizada de forma independiente y registrar solo resultado y cobertura, nunca credenciales, cookies, URL de conexión ni datos de tarea.
