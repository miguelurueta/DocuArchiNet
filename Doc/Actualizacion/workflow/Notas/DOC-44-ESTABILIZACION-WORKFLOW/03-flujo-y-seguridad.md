# Flujo y seguridad

1. El servidor evalúa el gate y el contexto válido.
2. Solo con gate activo renderiza el panel y registra el bootstrap con el `ClientID` del campo de tarea.
3. El cliente lee la tarea explícita, carga lista y contador y cierra el editor al cambiar de tarea.
4. El backend vuelve a autorizar cada operación; la UI no concede permisos.
5. Texto de usuario se asigna con `textContent`; guardar/eliminar se deshabilitan durante la solicitud.
6. `VersionConflict` obliga a recargar; no se sobrescribe silenciosamente.

El diálogo conserva foco, soporta Escape y Tab cíclico, anuncia estados por `role=status` y asocia ayuda/contador al textarea. El fallback mantiene su semántica histórica sin cambios.
