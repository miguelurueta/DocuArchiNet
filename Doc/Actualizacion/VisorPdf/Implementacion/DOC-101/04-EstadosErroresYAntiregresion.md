# Estados, errores y antirregresión

## Estados por capacidad

```text
no solicitada -> cargando -> lista
                    |
                    +-> fallida -> no solicitada -> reintento
```

Un fallo opcional muestra una alerta localizada, restaura el control y no altera PDF, página activa, scroll ni zoom. Los fallos HTTP o de transporte de las cinco familias se clasifican como `optional_resource`; un error de ejecución sigue causando `FAILED` para no ocultar regresiones.

## Contratos preservados

- Lectura, scroll, zoom, rotación y descarga.
- IDs y handlers de toolbar.
- Dropdown Bootstrap e iconos `fal`, `fad` y `far`.
- Firma personal, menú contextual, Wacom e impresión bajo permiso.
- Planificador por viewport de DOC-100.

## Rollback independiente

- Worker: retirar `workerSrc` y restaurar únicamente un mecanismo compatible de worker; nunca volver a ejecutar `pdf.worker.entry.min.js` en el frame.
- Propietarios: restaurar una familia a la vez (jQuery UI, Popper o Font Awesome) y ejecutar la matriz estática antes de mezclar versiones.
- Grafo, Wacom, impresión, Fabric o Prettify: retirar solo su entrada del manifiesto y restaurar temporalmente sus etiquetas previas, sin tocar otras capacidades.
- Configuración de sello: restaurar su llamada de arranque solo si una regresión funcional comprobada lo exige.

No hay rollback de datos, backend, transporte ni persistencia. La discrepancia Bootstrap JS 5.3.2/CSS 5.1.3 permanece como deuda fuera de alcance.

La matriz real validó el camino de Wacom no disponible y la recuperación del SDK. La captura sobre hardware queda como limitación ambiental documentada porque no había tableta conectada; no se reemplazó por una afirmación de captura física.
