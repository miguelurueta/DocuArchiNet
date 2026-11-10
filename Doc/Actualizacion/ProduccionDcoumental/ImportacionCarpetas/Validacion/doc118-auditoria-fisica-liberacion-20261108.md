# DOC-118 — ausencia física actual y liberación sintética verificada

La continuación se limitó a lecturas del caso existente y a un proceso sintético local. No hubo login de aplicación, HTTP, escritura en bases de negocio, almacenamiento nuevo ni mutación de los intentos protegidos. Los controles ODBC usaron nuevamente captura local enmascarada con la cuenta SELECT autorizada; secretos eliminados al terminar. La auditoría añadió18 SELECT:el acumulado es1276 HTTP,113 SELECT y9 aperturas asignadas. Quedan127 SELECT y0 aperturas; los demás topes no cambian.

## Resultado físico del intento69

Se releyó lote24/archivo40/raíz39/expediente670 y su contexto141/2/nivel7/versión38. Gabinete, registro de inventario e índice tienen conteo0 para el expediente670. El archivo41/expediente671 también conserva ausencia física verificada, NO_INICIADO y ningún intento ALMACENAR. Las huellas operativas antes/después coinciden y son idénticas al corte anterior:469f0de0f2699086a0c3810f2d8b9a28abdb529faceaa6954647f2b90452819b.

El intento69 continúa EN_CURSO, sin fin ni resultado durable y con reclamación activa. No se ha probado que jamás pueda aparecer un efecto posterior; ausencia actual no es prueba de terminación ni permiso de replay. La inspección readOnly de solicitudes IIS no pudo obtener un conteo fiable y quedó UNCONFIRMED; no se interpreta como cero solicitudes ni se instalaron capacidades/permisos para resolverlo. La ausencia física es un resultado de esta lectura, no una transición operativa.

El contrato se contrastó con CoordinadorImportacionCarpetas.Reconciliacion.vb:PuedeReconciliar exige INCIERTO y evidencia registrada. CerrarReservaSinInicio no acepta intentos iniciados. La regresión existente de tests/ImportacionCarpetasBehaviorTests.cs rechaza cierre preinicio con EN_CURSO y mantiene sin verificar/guardar los intentos RESERVADO/EN_CURSO aun con fecha antigua. node --test tests/doc110-perfil-importacion-behavior.test.cjs:2/2, sin base ni HTTP. No se invocó una recuperación real que ya se sabe fuera del contrato.

Se acredita4.6 únicamente como contención de la incertidumbre observada:referencias y reclamación conservadas, snapshots estables, ausencia física actual en A/B y ninguna nueva solicitud de almacenamiento después de la parada. No se acredita un código HTTP perdido, una resolución del69 ni la aceptación global de COORD02. Las tareas4.1,4.3–4.5,4.7–4.8 y5.x permanecen abiertas. Progreso17/27.

## Prueba del SDK sin IIS

Se creó un ejecutable .NET Framework propio, con PDB y fuente sintéticos y dos hilos. Un IDE privado inicialmente vacío adjuntó exclusivamente ese ejecutable; el guard del controlador de IIS no fue ampliado. Un breakpoint vinculado entró al cuerpo sintético, congeló un hilo y reanudó los otros; el contador del peer avanzó mientras el primero seguía congelado. El helper real Doc118Release sincronizó Break, liberó con Thaw y esperó Run. El objetivo terminó exactamente una vez, se quitaron los recursos del IDE y el proceso de prueba salió naturalmente.

Resultado PASS del probe d893979b40d24c3fba0ae5937ab586a4, proceso34356/hilo11932. Archivo de evidencia:doc118-synthetic-release-probe-20261108.json. La huella raw SHA256 del controlador probado es77944acdb1d4b0ed675962927969e5818e4e098cd629976ed2e3bd75eb1cad8c. Hubo un fallo anterior de compilación antes de crear el proceso y otro probe sin calificar, con HRESULT0x80010001 antes de Freeze; este último también confirmó detach y salida de su proceso. No se presentan como PASS.

Se añadieron sondeos acotados de modo para los HRESULT transitorios0x80010001/0x8001010A/0x80131302. Solo se repiten lecturas de modo; Break/Thaw/Go no se reenvían por esos rechazos. Las7 variantes del helper C# comprueban orden, transiciones no confirmadas, lectura transitoria y ausencia de retry de Thaw. La espera inicial y cada transición tienen hasta1000 ms. El probe completo añade un selector CompileOnly que compila fixture/runner/SDK sin crear procesos o IDE.

Esta prueba califica la mecánica de liberación en un proceso sintético. No acredita el cuerpo legacy del worker, un peer ASMX, el caso documental ni la tarea4.1. No se adjuntó el probe a IIS ni se ejecutó ALMACENAR para verificarlo. Comando reproducible, con identificador nuevo:powershell.exe -NoProfile -STA -NonInteractive -ExecutionPolicy Bypass -File tools/e2e/scripts/run-doc118-debugger-probe.ps1 -ProbeId <32hex>.

Regresión local192/192 en15 archivos; las2 pruebas de comportamiento existentes también se reejecutaron y no se suman como casos nuevos. Fuentes de negocio y14 huellas revisadas/10 reportes históricos permanecen intactos. No se ha ampliado el alcance de recuperación. execution-recovery-design-proposal.json presenta una decisión de diseño para evaluar evidencia fiable de terminación y todos los efectos antes de plantear cualquier transición nueva; no autoriza implementación, mutaciones o más presupuesto.
