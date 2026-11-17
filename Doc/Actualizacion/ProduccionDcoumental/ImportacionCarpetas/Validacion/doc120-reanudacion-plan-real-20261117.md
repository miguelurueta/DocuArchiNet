# Ventana propuesta: espera y reanudación general — DOC-120

Preparación de3.157; **NO EJECUTADA y SIN AUTORIZACIÓN OPERATIVA**. Complementa [capacidad](doc120-capacidad-plan-real-20261117.md) y la [ventana de3.86](doc120-coordinacion-plan-real-20261115.md). No habilita el resto del corpus ni hereda los5 SELECT cerrados de8803b6f4. Tampoco autoriza aplicar una DLL o reciclar el pool compartido.

## Recursos y presupuesto que deberán aprobarse

| Concepto | Propuesta concreta |
| --- | --- |
|Ambiente|Solo `https://localhost/GestionDocumental-Docuarchi.net/`; comprobar compilación y recursos compatibles antes de autenticar.|
|Cuenta|Propietaria141/empresa2 del lote50, con permiso actual en nivel223 y gabinete de su snapshot; segunda cuenta de privacidad solo si se aprueba repetir esa parte. Claves únicamente en consola privada; no guardar secretos.|
|Recurso|Mismo lote50/plan90/versión45. La observación previa de1.000 recibidos,4 guardados y996 pendientes no se toma como estado actual: nueva lectura acotada antes de actuar.|
|Efectos máximos|Una solicitud ReanudarLote y, después de acuse/consulta coherentes, **un** documento pendiente almacenado por el protocolo existente. Cero confirmaciones nuevas, recepciones, expedientes, materializaciones nuevas y borrados. No solicitar los995 restantes. Si cambió el inventario/estado, detener y revisar el plan.|
|Topes externos|600 HTTP,120 SELECT de control,2 sesiones,45min de ventana. Son límites propuestos de la prueba; no una cancelación automática a30s del documento. Contabilizar acumulado de toda continuación. No incluye SELECT internos de la aplicación, que se medirán por separado.|
|Bytes/disco|0 bytes de nuevos PDF recibidos. Medir antes el tamaño del PDF pendiente ya recibido y destinos; autorizar como máximo ese PDF y un crecimiento combinado de derivados/temporal de64MiB. Si el tamaño excede el presupuesto o no puede medirse, no almacenar.|
|Mediciones|Tiempos por las fronteras de3.153, referencia de operación y resultados; archivo/identidades solo en evidencia local saneada. No HAR, cookies ni cuerpos PDF/SQL.|
|Interrupción|Pausa local al iniciar el único almacenamiento permitido; esperar respuesta y confirmar que no comienza otro. Cerrar/recargar después de la pausa y consultar el mismo lote. No desconectar, abortar writer, matar procesos ni alterar estados por SQL.|

## Ejecutor y secuencia

Antes de ejecutar, adaptar/revisar el runner existente con allowlist de `ReanudarLote`, presupuesto acumulado y guardia de **un solo almacenamiento**. Debe pedir la pausa mediante el coordinador al observar ese primer envío, dejarlo terminar y bloquear cualquier segundo efecto. No usar un runner antiguo ni confiar en que una persona alcanzará a pausar entre respuestas rápidas. Preparar este plan no acredita que dicho runner esté listo o que la ventana esté autorizada.

1. Verificar versión desplegada, conexiones efectivas y autoridad. Lectura RA/DA de control acotada al lote y expediente conocido. Si hay incertidumbre, actividad o revisión distinta de la elegible, cerrar sin efectos; nunca inferir abandono por antigüedad.
2. Comprobar recibos y cuatro identidades documentales previas. Consultar/filtros/CSV no ejecutan reanudación. Registrar revisión y confirmaciones antes del control.
3. Consentimiento explícito y una ReanudarLote con revisión vigente. Verificar evento, señal, estado derivado y aumento de revisión; acuse perdido permite únicamente consultar y nueva decisión, no reenviar automáticamente ni arrancar la cola.
4. Dejar que el coordinador seleccione por ID el primer pendiente. La guardia de runner permite solo ese almacenamiento; pedir pausa y esperar su respuesta, aunque exceda30s. Distinguir fallo HTTP/IIS de espera aún pendiente. Registrar etapas y acuse del checkpoint.
5. Comparar identidades iniciales, total1.000, recibos y nuevo documento confirmado. Si tuvo éxito, esperar5 guardados/995 pendientes; sin segunda escritura. Cerrar/recargar y consultar sin consentimiento heredado. Control antes/después de consultas debe preservar evidencia.
6. Cerrar ventana y eliminar secretos. Primer error, falta de acuse, cambio de autoridad, presupuesto, incoherencia o intento incierto: STOPPED sin replay. Conservar referencias para diagnóstico y consumo acumulado.

## Casos que requieren otra ventana

Carrera real entre dos pestañas y fallo/rollback MySQL se probarán en un lote de laboratorio separado: dos controles con la misma revisión, un ganador/evento; nueva detención invalida una revisión vieja; excepción entre actualización/evento revierte todo. Esta propuesta sobre lote50 **no** autoriza inyectar fallos ni crear ese laboratorio. Los modelos transaccionales locales no cierran3.118 ni certifican esos bloqueos reales.

La carga completa de1.000 PDF sigue en3.126/3.127 y el plan de capacidad anterior. No mezclar sus efectos/topes con esta ventana de un documento. No cambiar Web.config, escritor compartido, política de integridad, SII, jobs o limpieza. [Runbook](../../../../../tools/e2e/AGENT-RUNBOOK.md).
