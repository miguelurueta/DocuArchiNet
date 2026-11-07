# DOC-111: persistencia del resultado de creación, 2026-10-29

Avance parcial de 10.7. **97/130 tareas completas, sin incremento**: se registra la fase de creación; almacenamiento/incorporación, coordinación, reconciliación y aceptación real siguen pendientes. No se ejecutan efectos documentales o escrituras MySQL reales.

El parcial ResultadoCreacion del repositorio único implementa RegistrarResultadoCreacion. Reutiliza Operar para transacción/commit y el orden de bloqueo lote→raíz→intento. La lectura documental del intento se comparte con la repetición de reclamaciones, sin otro repositorio o tabla. El resultado debe corresponder al intento/ciclo de inicio, token/huella, ámbito/snapshot y raíz. Las salidas de otras fases y las identidades malformadas se rechazan.

El resultado se canoniza antes de guardar: código/mensaje internos conocidos, sin errores libres del escritor. Confirmado exige ambos IDs, guarda evidencia del intento y la identidad de la raíz, cambia a EXPEDIENTE_CONFIRMADO y libera el marcador. Incierto conserva IDs parciales en evidencia, deja identidad de raíz sin confirmar, mantiene el marcador y cambia el lote a PENDIENTE_VERIFICACION. No cierra el lote ni reduce totales. Una señal de detención recibida en vuelo se conserva y no impide registrar el efecto ya producido.

Los tres UPDATE y el evento pertenecen a una transacción propia posterior a legacy. Cada actualización compara estado/revisión/identidad de recurso esperados. Una actualización de cero filas o fallo del evento propaga el error al contenedor transaccional. La repetición exacta del resultado canónico ya guardado no escribe ni agrega evento; no cambia IDs conocidos ni transforma INCIERTO a CONFIRMADO. Esos cambios requieren reconciliación explícita futura.

El adaptador registra todas las salidas posteriores al inicio. Inicio, verificación y registro usan el mismo repositorio/origen capturado en la composición productiva. Si falla el checkpoint o se pierde la respuesta después de un commit, devuelve INCIERTO e IDs conocidos sin otra invocación. Registrar evidencia no añade un nuevo efecto documental ni simula una autorización para reintentarlo.

Corrección del corte anterior: IniciarCreacionExpediente usaba resultado_doc; el DDL de intento y sus lectores definen evidencia_doc. Se corrige a evidencia_doc sin alterar esquema o datos. La nueva regresión cruza las columnas SET de los UPDATE de intento contra el DDL operativo. El corte previo solo compiló/probó dobles, por lo que no se atribuye un éxito SQL real a aquel enlace incorrecto.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: dos pruebas Node, arnés Framework con familias previas y nueva comprobación de columnas contra DDL. Ejecutor SQL sintético para confirmado/incierto, IDs parciales, señales de detención, repetición sin escrituras, resultado cambiado, alcance/token/huella/revisión ajenos, prueba previa discordante y datos malformados. Fallos en cada UPDATE detienen el flujo; el fallo de evento se propaga. No se certifica rollback o bloqueo MySQL con ese doble.
- Adaptador con doble de registro: fallo antes de guardar y pérdida de respuesta después de guardar conservan incertidumbre/IDs, no filtran los mensajes centinela ni repiten el escritor.
- Build Debug completo sin despliegue; salida/intermedios `.tmp/doc111-resultado-creacion-build/{bin,obj}`, log `.tmp/doc111-resultado-creacion-build.log`, código 0.

No se probaron persistencia/rollback/carreras reales del nuevo SQL, IIS/autenticación, XML físico, creación o almacenamiento reales. Sin cambios de configuración/despliegue. ClassGaExpediente, ClassGaProducionDocumental, ClassAlmacenamiento, ClassNeodynamic y conect permanecen intactos; SII y carga individual preservados.
