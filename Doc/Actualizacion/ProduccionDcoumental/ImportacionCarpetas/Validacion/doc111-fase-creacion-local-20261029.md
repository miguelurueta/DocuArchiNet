# DOC-111: una fase interna de creación coordinada, 2026-10-29

Avance parcial de 10.3–10.7 y controles de detención/autoridad. **97/130 tareas completas, sin incremento**. La fase de creación está compuesta y probada localmente; no completa las fases documentales restantes ni habilita un endpoint o ejecución desde el cliente.

CrearExpediente del coordinador reconstruye identidad/destino desde el lote propio y consulta una vista del plan sellado. Genera token en servidor y huella estable de contexto/lote/raíz/revisión/clave/fase. ObtenerIntentoCreacion del mismo repositorio consulta la reclamación histórica bajo ámbito autorizado y siempre devuelve EsNuevaReserva=False. Una repetición informa el intento sin reservar, rehacer staging o volver a ejecutar.

Para una operación nueva, el coordinador prepara la recepción física, reserva una vez, vuelve a cargar raíz/lote y revalida marcador/revisión/detención/autoridad antes del creador. No pasa a otra raíz, almacenamiento, incorporación o diferidos. Una reserva que no puede iniciar queda visible; no se cancela o libera para simular ausencia de efectos.

La comprobación física anterior liberaba cada lectura antes de reservar. Ahora ConservarVerificado del staging existente abre el archivo privado con FileAccess.Read/FileShare.Read, valida bytes/hash/PDF y devuelve una retención IDisposable sin ruta o stream público. El contenedor de la unidad conserva los archivos válidos y cierra todos sus handles al terminar o fallar. En parcial puede conservar otro archivo válido y los IDs inciertos sin reducir inventario.

El binding de creación exige el callback de retención, lo obtiene después del inicio durable, reautoriza y mantiene los originales protegidos durante invocación, verificación y checkpoint. Siempre libera en Finally. La relectura previa por sí sola no se convierte en un permiso durable; la retención cubre la ventana del efecto síncrono. No se afirma aislamiento del almacenamiento legacy, que sigue sin modificarse/habilitarse.

Tras la llamada, la fase consulta el registro persistido. Solo informa CONFIRMADO con intento/resultado confirmados, fin registrado, IDs completos y la misma identidad de raíz; una respuesta perdida después del commit se recupera mediante lectura. Sin checkpoint concluyente conserva INCIERTO y los IDs observados válidos, sin nueva ejecución. Códigos/mensajes privados del escritor no se propagan.

ComposicionCreacionImportacion compila la composición real con las clases existentes y exige la petición Current. Las políticas y presupuestos PDF siguen siendo explícitos; el parser se comparte con recepción sin cambiar sus valores/comportamiento o Web.config. La composición y el binding reales no se ejecutaron en el arnés.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: arnés Framework, familias previas y regresión DDL existente. Fase compuesta con PDFs locales, coordinador/adaptador/checkpoint reales y dobles explícitos de consultas, persistencia y escritor. Un único efecto, un checkpoint y repetición histórica sin nuevo staging/reserva/escritor.
- Casos de detención/revocación después de reservar, staging perdido entre reserva e inicio, error del escritor, checkpoint fallido y respuesta de commit perdida. Se conservan incertidumbre/IDs y no se filtran los centinelas privados.
- Handles locales: escritura y borrado del original bloqueados mientras se conserva; cierre posterior permite volver a abrir. Si falla un segundo archivo se cierran las retenciones anteriores. El doble del escritor comprueba que el original permanece retenido hasta el checkpoint.
- Build Debug completo sin despliegue, salida/intermedios `.tmp/doc111-fase-creacion-build/{bin,obj}`, log `.tmp/doc111-fase-creacion-build.log`, código 0.
- OpenSpec estricto y revisión de diferencias PASS; funciones ClassGaExpediente, ClassGaProducionDocumental, ClassAlmacenamiento, ClassNeodynamic y conect sin cambios.

Límites: no MySQL/HTTP/IIS/legacy reales, XML físico o E2E autenticados. La retención se verificó en archivos temporales locales; no certifica todos los ambientes/filesystems. No se implementan aún las fases documentales restantes, reconciliación general, exposición HTTP o continuación cliente. No se alteran SII, carga individual, configuración o despliegue.
