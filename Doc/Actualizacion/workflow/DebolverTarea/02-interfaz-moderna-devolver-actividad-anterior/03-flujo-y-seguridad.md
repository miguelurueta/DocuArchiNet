# Flujo, seguridad, accesibilidad y relevo

## Recorrido de usuario

1. La persona elige **Elegir actividad anterior**. El modal toma el identificador de la tarea seleccionada y solicita el preview oficial.
2. El modal muestra el contexto resumido y los destinos permitidos en tabla de escritorio y tarjetas para pantalla reducida. Puede buscar, avanzar o volver de página.
3. Al elegir un destino, la selección vigente abre `ConfirmationDialog`; solo esta selección contiene tarea, conector y token.
4. La confirmación ejecuta `EjecutarDevolverActividad`. Mientras está pendiente, el diálogo compartido bloquea repetición, confirmación y cierre que pueda abandonar el resultado.
5. Ante éxito, `WorkflowTransitionPagePresentation.applySuccess` actualiza solo la tarea correlacionada, sus contadores y la posición de lista. Ante bloqueo o error, la UI conserva un mensaje funcional saneado y no modifica la tarea.

## Accesibilidad y responsive

El diálogo declara `role="dialog"`, `aria-modal`, título y descripción propios; al abrir guarda el foco anterior, lo mueve al cierre y aplica trampa de foco. Escape, botón cancelar, botón de cierre y clic de fondo cancelan el preview sin ejecutar nada. Tabla y tarjetas se alimentan de la misma página normalizada, evitando divergencias visuales. Los estilos conservan encabezado y cierre visibles y dejan el cuerpo desplazable en tamaños limitados.

## Seguridad y no regresión

Preview no equivale a autorización de ejecución. Un token, conector o selección vencidos se invalidan en el cliente y se vuelven a validar dentro del lock del servidor. Los errores visibles no incluyen SQL, excepción, URL sensible, sesión, cookies ni credenciales.

Se retiraron el trigger `D-TASK-ANT`, el botón `Button_tool_devolver_a_actividades_anterior`, su diseñador, su handler Web Forms y el callback de postback asociado. `Usuario anterior` y los contratos de Enviar a usuario, Enviar a grupo y Continuar flujo permanecen separados. La reversa consiste en retirar los assets/markup DOC-33 y restaurar desde control de versiones; no requiere migrar datos ni revertir transiciones confirmadas.
