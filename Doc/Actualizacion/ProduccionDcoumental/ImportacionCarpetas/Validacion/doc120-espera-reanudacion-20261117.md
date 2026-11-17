# DOC-120 — espera de respuesta y reanudación general, 2026-11-17

Base6ae46470; instrucción «continua con la implementacion».3.136–3.158 implementadas/verificadas dentro del alcance local/documental: **167/178 tareas**; once anteriores abiertas. [Contrato](../Diseno/espera-respuesta-reanudacion-lote-doc120.md). [Fuentes, huellas y resultados](doc120-espera-reanudacion-20261117.json).

## Comportamiento entregado

- Recepción multipart y AvanzarLote/ALMACENAR usan XHRtimeout0. La cola espera acuse y lectura coherente antes del siguiente efecto. Consultas, creación y controles conservan su timeout previo; no se cambia Web.config ni el writer compartido.
- Bytes transmitidos100% se distinguen de recepción confirmada. La vista conserva fase, archivo/ruta, posición y último avance. Pausar espera la solicitud iniciada; cerrar invalida la continuación tardía.
- ReanudarLote recibe lote/revisión por el ASMX existente con sesión/CSRF y autoridad vigente. El repositorio bloquea primero lote, relee inventario/actividad/evidencia y actualiza estado/señal/revisión/evento en su transacción. No ejecuta recepción, creación o almacenamiento.
- La consulta anuncia REANUDAR_LOTE/motivoBloqueo. Solo el clic explícito, seguido de acuse y lectura no anterior a ese acuse, inicia la coordinación existente. Acuse perdido, nueva detención, actividad, incertidumbre o servidor anterior conservan consulta y evitan continuación automática. Retomar creación mantiene su contrato específico.
- Medición saneada de fronteras del importador, sin nuevas tablas/columnas, configuración de trazas o endpoint de diagnóstico. La [propuesta de optimización](../Diseno/medicion-optimizacion-almacenamiento-doc120.md) no elimina barreras ni se aplica como parte de esta entrega.

## Verificación local

**134/134 pruebas Node únicas:**132 cliente/Chromium y2 wrappers Framework/DDL. No se suman los casos internos C#/VB a ese total. Comandos:

```text
node --test tests/doc120-espera-reanudacion.test.cjs tests/doc120-parada-diagnostico.test.cjs tests/doc120-coordinacion-core.test.cjs tests/doc120-coordinacion-browser.test.cjs tests/doc120-importacion-mensajes.test.cjs tests/doc120-importacion-flujo-final.test.cjs tests/doc120-preparacion-tamano.test.cjs tests/doc111-importacion-carpetas-client.test.cjs tests/doc120-segmentada-browser.test.cjs
node --test tests/doc110-perfil-importacion-behavior.test.cjs
```

Transporte con reloj sintético supera31s; solo el acuse resuelve. Chromium cubre recepción/guardado pendientes en390/768/1280px, pausa sin abortar y cierre sin siguiente efecto. El escenario equivalente a1.000 recibidos/4 guardados retoma ID82 del inventario sintético y confirma el quinto; no reenvía PDF, confirma otro plan ni crea otro expediente. Respuesta perdida, lectura anterior al acuse, nueva detención y servidor anterior no arrancan otro recurso. Reabrir consulta no hereda consentimiento.

El arnés Framework prueba recepción pendiente/LISTO/EN_CURSO/COMPLETADO, conservación de resultados, bloqueo por actividad/incertidumbre/marcador, estados terminales, permisos/empresa/propietario/destino, CSRF/campos ajenos y cuerpo/envelope ASMX. Dos solicitudes con la misma revisión producen un ganador/evento en el modelo transaccional. Una revisión vieja no levanta otra detención; fallo de actualización/evento no se aplica en ese modelo. **No equivale a probar locks o rollback MySQL reales.** Las pruebas existentes de creación y recuperación V2 siguen incluidas.

La regresión cliente conserva selección/perfiles, coordinación, mensajes, filtros/paginación/CSV/privacidad sintética, árbol y preparación/consulta focal con1.000 archivos. Capturas locales en `.tmp/doc120-coordinacion-ui/espera-RECEPCION-*.png` y `espera-ALMACENAR-*.png`. Son fixtures con módulos reales y transporte sintético, no capturas autenticadas del IIS.

Proyecto VB compilado con MSBuild, targetCompile y OutputPath/OutDir/IntermediateOutputPath aislados en `.tmp/doc120-espera-reanudacion-build/`: cero errores; advertencias del proyecto conservadas. OpenSpec estricto válido,62 escenarios, auditoría de refinamiento15/15 y diff sin errores de espacios. Los logs locales se referencian en el JSON; no se versionan binarios, staging o datos de ejecución.

## Medición y límites

Línea base1/100/1.000:2/200/2.000 cargas y comprobaciones por operación de dos pasadas en completo,2/2/2 en parcial. Una raíz; EXPEDIENTE y LOTE comparten inventario aquí. Dobles de metadatos/staging:0 SELECT reales,0 bytes físicos medidos,0 writer real. Los tiempos y bytes representados del laboratorio se conservan separados en el JSON; no certifican latencia de IIS ni la causa histórica de la parada.

**No desplegado:** DLL compilada SHA-256 `c74f759871fa4b6899ced0bdd36c09c9128590b7c1ea6358cb8f50a5244e427c`. La DLL activa conserva `5b821504cf0cb583d4cc5dbd155e8eafad50a2a21196bc06b2749c312b77fff4` del corte beae0afc. Los JS/ASPX de la carpeta compartida sí cambian sus fuentes y cache token; el servidor anterior no anuncia REANUDAR_LOTE y la interfaz ofrece consulta. La capacidad nueva requiere desplegar una pareja compatible de servidor/cliente.

Este corte realiza0 HTTP al ambiente real,0 SELECT reales,0 sesiones autenticadas y0 efectos reales. No consulta otra vez ni levanta el lote50; su diagnóstico anterior queda intacto. No certifica originales físicos, documentos en DA, rendimiento o aceptación operativa. [Ventana propuesta de una reanudación/un documento](doc120-reanudacion-plan-real-20261117.md), sin ejecutar. No autoriza publicar, fusionar, cerrar Jira o archivar DOC-120.

Permanecen3.86/3.87/3.118/3.126/3.127/3.130/3.131/5.2/6.4/6.5/6.6. La ampliación de longitud de Egresos conserva su estado anterior; implementar estas tareas no demuestra que aquel ajuste se haya aplicado.
