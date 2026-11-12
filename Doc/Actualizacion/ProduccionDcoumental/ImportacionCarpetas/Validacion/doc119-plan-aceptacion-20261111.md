# DOC-119 — plan de aceptación para concretar y autorizar

Preparación de tarea 1.2, D-10/RQ-10. Este documento no autoriza ejecución real. La tarea no se completa hasta fijar ambiente/cuentas/recursos del responsable; se solicitaron referencias sin secretos durante la sesión.

## Destino propuesto y datos que faltan

El antecedente técnico DOC-118 declara `https://localhost/GestionDocumental-Docuarchi.net/`. Es propuesta de ambiente para revisión, no autorización vigente. Faltan cuenta propietaria, cuenta ajena autorizada para el negativo y referencias de lotes para cada caso. No inferir esos datos de permisos/sesiones históricas.

SEG-01 necesita lote del propietario con estados mixtos y suficientes filas para más de una página según política vigente; al menos un filtro con coincidencias y uno sin ellas. Lote5 histórico PARCIAL es una referencia candidata para revisar por SELECT si el responsable la elige; no se presupone propietario, disponibilidad o adecuación actuales. Lotes27/28 y restantes recursos protegidos se conservan; este plan no los modifica ni reejecuta.

SEG-02 necesita un lote existente con recepción por completar, archivo local correspondiente autorizado para reselección y rama original visible/cargada; además otro nivel seleccionable y caso homónimo. Si no existe recurso adecuado, su creación/carga requiere propuesta y autorización separadas. No fabricar datos, modificar estados ni repetir efectos inciertos para construir el escenario.

## Ventanas propuestas

| Ventana | Acciones revisables | Techo propuesto, sin autorización concedida | Criterio de aceptación |
| --- | --- | --- | --- |
| SEG-01 | Dos sesiones: propietario y cuenta ajena; ConsultarLote, ConsultarRecursosLote, ExportarPaginaLote y descarga desde vista | 2 aperturas de sesión, 120 HTTP de aplicación, 80 SELECT; sin efectos documentales | Filtros/páginas/CSV coinciden, totales originales preservados, fórmulas neutralizadas, cuenta ajena sin filas/CSV y huellas de negocio invariantes |
| SEG-02 lectura/UI | Recarga, ConsultarConfirmacionLote si corresponde, consulta del lote, reselección local sin envío y actualización deduplicada de rama | 2 aperturas de sesión, 80 HTTP de aplicación, 60 SELECT; cero ConfirmarPlan/AvanzarLote/ReconciliarRecurso/upload | Destino del lote y selección conservados, referencias aisladas, homónimos identificados por IDs y ausencia de confirmación repetida |
| SEG-02 recepción, solo si resulta necesaria | Enviar únicamente el recurso pendiente y contenido fijados después de revisar lectura/UI | No concedido; requiere IDs, PDF/hash/tamaño y presupuesto de un envío acordados aparte | Verificación servidor por contenido/autoridad sin duplicación; ningún almacenamiento o efecto adicional implícito |

Los techos propuestos cuentan reintentos y fallos de sesión; al agotarlos se detiene sin reiniciar contador. La sesión genera los registros normales de login/apertura existentes; invariancia se evalúa sobre datos/auditoría de negocio de los recursos, no se promete ausencia absoluta de registros de acceso.

## Controles y reutilización de herramientas

Reutilizar sesión autenticada y puente ODBC `queryRows` existentes. Los DSN RA/DA y consultas SELECT parametrizadas se fijan tras identificar recursos y revisar las tablas vigentes; no se admiten cadenas de conexión o secretos en el perfil. Huellas antes/después de lote/raíces/archivos/intentos y relaciones documentales pertinentes; sin UPDATE/DELETE, FOR UPDATE ni funciones de espera.

`run-doc111-importacion-carpetas.cjs` predeterminado ejecuta REC con efectos. `--diagnose` solo diagnostica acceso y no acredita seguimiento; `--verify-lot` está restringido a lote COMPLETADO y no acredita filtro/CSV/negativo/recarga incompleta. Los modos de recuperación pueden cerrar reservas, reconciliar y continuar almacenamiento: quedan fuera de las ventanas de lectura.

`run-doc113-ventana-importacion.cjs` cubre los casos UI-01/02/03 de DOC-113; UI-02 prepara planes y puede confirmar si se solicita. No es un selector SEG-01/02. Antes de una corrida DOC-119 hay que revisar/adaptar el recorrido de seguimiento con las mismas utilidades y una lista cerrada de operaciones permitidas; no ejecutar otro escenario para simular aceptación.

## Detención y evidencia

Detener ante autoridad/identidad/destino discordantes, operación fuera de la lista, mutación inesperada, respuesta no interpretable, control SELECT fallido o presupuesto agotado. No reintentar automáticamente ni convertir incertidumbre en permiso de recepción/almacenamiento. No cambiar permisos para probar revocación sin autorización específica.

Conservar solo resultado saneado, SHA/hashes del cliente servido y ejecutor, ambiente, IDs autorizados, contadores, códigos y huellas. Capturar secretos por consola compartida y eliminarlos al cerrar; nunca guardarlos o imprimirlos. No borrar lotes, temporales o evidencia al terminar.

Falta la selección del responsable y autorización expresa de estas ventanas antes de autenticación o consultas reales. La reorganización UI de la sección 3 permanece detrás de la aceptación integrada SEG-01/02; preparar este plan o pasar pruebas locales no abre esa compuerta. SEG-03 requiere decisión propia y no habilita borrados.
