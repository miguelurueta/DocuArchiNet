# DOC-118 — adaptador V2, RV-03

Implementada6.3/D-13/RQ-13 tras «continua» posterior a RV-02-B y al siguiente paso RV-03. Se incorporó la tarea al mapa antes de cambiar código. El avance queda20/30 por tres tareas locales nuevas completadas respecto de las27 originales; las10 tareas reales/finales anteriores siguen pendientes.

El parcial `AdaptadorDocumentoImportacion.ProtocoloV2.vb` añade un recorrido interno al mismo adaptador. Reutiliza autorización, catálogo, copia, escritor y verificador existentes. Exige el proveedor V2 y un puerto obligatorio de persistencia, sin usar inicio/checkpoint V1 como fallback. La composición pública, coordinador, repositorio y HTTP permanecen V1:RV-04 debe implementar la persistencia y su diario transaccional antes de activarlos.

La retención se adquiere antes del inicio y se conserva durante autorización, copia, invocación, observación, verificación/checkpoint y salida. Una respuesta perdida, nula o incoherente de inicio/autorización no concede invocación. La instancia consume una única ejecución mediante Interlocked y no reintenta callbacks. Esto no sustituye el lock durable entre instancias que debe implementar RV-04.

Al salir del escritor, finally conserva IDs y realiza un intento de registrar una observación tipada. IDs desconocidos permanecen ausentes. La observación es INCIERTO; la confirmación física ocurre después. Sin acuse exacto de observación no se intenta verificar/checkpoint. Una respuesta perdida del checkpoint preserva IDs y devuelve incertidumbre. Cancelación y ThreadAbort se propagan, sin ResetAbort; la retención se libera en finally.

Los acuses son internos y no se deserializan desde JSON/HTTP. El tipo no demuestra por sí solo un commit:el puerto de persistencia debe emitirlo solo tras commit confirmado. Esta entrega prueba el contrato con dobles explícitos y no acredita atomicidad MySQL ni una llamada legacy real.

Verificación:

-20 casos nuevos del adaptador dentro del harness .NET Framework existente:orden y retención, inicio/gate perdido/nulo/alterado, autoridad revocada, original ausente, escritor fallido, IDs parciales, observación incierta, verificación negativa, checkpoint perdido/alterado, repetición/concurrencia, cancelación, ThreadAbort, acuse interno y privacidad.
- Se usa el staging real sobre PDF sintético; persistencia y escritor son dobles. Un hilo propio se aborta y termina; no se toca IIS ni un worker de aplicación.
- Se conservan19 casos de contratos y21 del proveedor, con3 procesos sintéticos cerrados. Los casos internos C# no se suman al contador Node.
- Regresión Node18/18 de comportamiento/cliente y205/205 de herramientas:223 disjuntas actuales. OpenSpec, refinement y diff se verifican sobre la entrega.
-12 fuentes históricas permanecen idénticas y2 archivos de pruebas conservan deltas antes/después.10 informes históricos y16 fuentes de la evaluación permanecen intactos; los adaptadores V1/legacy y el almacenamiento compartido no se modifican.

No hubo nuevas sesiones, HTTP de negocio, SELECT o efectos documentales reales. Consumo1276 HTTP/113 SELECT/9 aperturas e intentos41/53/69 protegidos. El límite de misma época del proveedor sigue vigente; otro nodo o reinicio mantiene bloqueo. No hay despliegue, DLL/PDB, migraciones, permisos o configuración cambiados. Sigue RV-04.

Evidencia:[JSON local](doc118-adaptador-v2-local-20261108.json).
