# DOC-116 — contrato de prueba del permiso de nivel

Columna única: agregar_expediente en ra_pro_permisos_niveles. Destino/actor: IDs canónicos del escenario revisado; no propietario del nivel y misma empresa. La lectura de control exige una sola concesión activa y nivel activo. IMPORTAR_CARPETAS/ADMINISTRAR_PERFILES_IMPORTACION no sustituyen este permiso.

Antes de mutar: SELECT de actor/empresa/nivel/propietario/concesión efectiva y clave real de fila; abortar ante cero/múltiples filas o propiedad. Conservar en diario restringido los IDs, valor previo exacto, huella de columnas ajenas, alcance autorizado y estado PREPARADO. No incluir credenciales/conexiones. El responsable debe autorizar una cuenta de escritura separada; la cuenta de control conserva solo SELECT.

Secuencia de mutación implementada y verificada solo localmente: comprobar el diario y el estado previo mediante transacción; cambiar solo agregar_expediente de 1 a 0 con comparación de identidad/valor previo; exigir una fila; commit y lectura independiente; registrar REVOCADO. Ejecutar una única petición de creación ya presupuestada, comprobar denegación y conteos de expedientes/relaciones sin cambios. No repetir creación ante error de transporte o respuesta no clasificada.

Restauración en Finally: comparar fila/valor revocado y huella ajena, restaurar exactamente el valor previo, comprobar una fila/commit y SELECT independiente. Registrar RESTAURADO solo con lectura exacta. Ante cambio externo o restauración no confirmada, conservar diario y detener; no forzar sobrescritura ni anunciar PASS. Recuperación tras cierre de consola debe consultar diario/estado antes de actuar y no repetir la creación.

Estado actual: mutador y recorrido EXP03 implementados; verificación local con memoria, comandos/transacciones dobles y diario físico. Prueba real NO_EJECUTADO. Se agrega un modo cerrado al puente administrativo local, sin importar la extensión operativa de main ni cambiar su modo anterior. Exige tablas InnoDB, identidad de fila y fingerprint del ambiente revisados; ante incompatibilidad no muta ni aplica DDL. Falta seleccionar actor no propietario/nivel/lote, comprobar referencias del ambiente y registrar autorización específica antes de ejecutar.

Actualización 2026-11-05: referencias candidatas seleccionadas por diagnóstico SELECT (actor 141/empresa 2, nivel 215 no propietario, permiso 629). Setup y factoría compartida compuestos; 95/95 pruebas locales PASS. La cuenta escritora es la conexión operativa del puente administrativo existente, sin pedir credenciales nuevas de escritura ni utilizar la cuenta de control. Autorización específica y ejecución real todavía pendientes.

Ejecución real revisada 2026-11-05: EXP03 PASS, plan 34/lote 18/nivel 215/fila 629. Una creación denegada y cero efectos de creación; restauración y readback comprobados, diario RESTORED. Evidencia completa: `doc116-exp03-resultados.md`. No repetir revocación ni creación.
