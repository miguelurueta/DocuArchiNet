# DOC-88 — Impacto de interfaz

La única proyección modificada es `insert_row_producion_documental` cuando el componente fue configurado con `evento_adjunta: "PRODUCCION"`.

## Flujo

```text
[Usuario pulsa Guardar]
          |
          v
¿PRODUCCION ya tiene una solicitud activa?
     | sí                         | no
     v                            v
[Ignorar segunda activación]  [Enviar archivo una vez]
                                   |
                                   v
                         ¿respuesta válida y YES?
                           | no             | sí
                           v                v
                  [Mostrar error]   [Validar datos de fila]
                                             |
                                             v
                                  ¿IDs y nombre presentes?
                                     | no           | sí
                                     v              v
                         [PRODUCCION_CARGA_   [Insertar fila con
                          PROYECCION_FALLIDA]  id_image real]
```

No se introduce recarga, postback, temporizador de consistencia ni segundo envío. La bandera se libera en `finally`, tanto en éxito como en rechazo.

Los controles visuales, foco, estilos, tabla, modal y accesibilidad no cambian. La validación manual queda pendiente de una E2E autorizada.
