# Arquitectura y componentes — Lista preview

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Arquitectura de la solución

La lista preview pertenece a `workflow-user-send-ui.js`. Se abre desde `workflow-user-send-trigger`, consulta una página de destinos al ASMX y dibuja la misma colección en tabla de escritorio y tarjetas móviles. El adaptador conserva término, página, cursores, `AbortController` cuando existe y una secuencia monotónica que elimina resultados tardíos.

`PreviewEnviarUsuario` y `ServicioEnvioUsuarioTarea.Previsualizar` pertenecen a DOC-28. El navegador no construye destinos, no evalúa permisos y no conoce la consulta MySQL. El repositorio resuelve únicamente parejas usuario–actividad vigentes para la tarea y ruta actuales.

## Alcance y compatibilidad

La lista preview no reemplaza la confirmación ni ejecuta transiciones. Emite una selección con usuario, actividad y token para que el componente de confirmación decida la ejecución. `WorkflowTransitionUi`, Grupo y Continuar flujo no comparten cursores, eventos, estado, selectores ni `IdConector`.

La página se mantiene dentro del mismo modal de usuario. La altura estable y el scroll interno evitan que cargar, vaciar o pintar resultados modifique la posición del diálogo.
