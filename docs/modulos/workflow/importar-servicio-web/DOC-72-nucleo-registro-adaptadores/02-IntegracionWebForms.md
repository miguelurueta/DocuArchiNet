# Integración WebForms

`workflow/Webworkflow.aspx` contiene el modal aditivo. `Webworkflow.aspx.vb` registra CSS y scripts en orden API → registro → core → UI solamente después de validar `WorkflowCentroTrabajoModernActive`. El bootstrap entrega tarea y proveedor configurado mediante atributos `data-*` codificados.

Con gate apagado no se cargan los assets y `btnloadservice` conserva su comportamiento. Con gate autorizado, `ctw-document-action-service` abre el modal y el puente legacy se oculta visual y semánticamente.
