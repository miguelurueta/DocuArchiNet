# Diagnóstico previo al escritor: lote50

Motivo: HTTP500/ERROR_INTERNO en ALMACENAR107, run42318523, tras instalar el procesamiento independiente. La comprobación posterior de RA conserva revisión3057/10 completados/990 pendientes sin reserva activa. La excepción exacta no está registrada en la evidencia anterior.

La cobertura local anterior verifica procesamiento individual con un puerto que ya expone la cabecera de control; no ejecuta la inicialización completa del repositorio contra registros existentes. No se convierte esa cobertura en ausencia de regresión real. La tarea3.199 incorpora un diagnóstico separado; no modifica la aplicación ni sustituye el fallo por una declaración de éxito.

## Auxiliar y alcance

`inspect-doc120-independent-preflight.cjs` reutiliza la consola privada y el puente SELECT existente. Máximo7 SELECT RA, siempre lote50/propietario141/empresa2/nivel223. Lee lote, plan, fragmentos, raíz, archivos, intentos relevantes y de nuevo lote/último evento. Sin HTTP, sesión web, FOR UPDATE, escrituras, originales en disco, reserva ni escritor. Plan y valores viajan por memoria/stdin; no se conservan en archivos o argumentos. Solo se guarda diagnóstico cerrado y consumo; claves eliminadas al terminar.

`Doc120IndependentPreflight.cs` compila contra la DLL instalada y llama mediante una lista cerrada de métodos estáticos a los lectores reales `LeerPlan`, `ReconstruirPlanSegmentadoPersistido`, `LeerRaizControl`, `LeerArchivoRecepcion` y `LeerIntentoDocumental`. Después ejecuta `EvaluarConsolidacion`, `ControlArchivoIndependienteImportacion.Sellar`, serialización de controles y verificación de pruebas en memoria. No instancia el repositorio ni dispone de una conexión o adaptador documental.

Fallo: conserva fase, tipo de excepción, métodos del ensamblado y código controlado. No imprime Exception.Message, stack completo, valores, rutas de negocio o JSON de entrada. Éxito: acredita exclusivamente estas lecturas/validaciones/cálculos; no ejecuta ni certifica inserciones de controles, aislamiento, commits, autorización de sesión, staging o almacenamiento.

## Verificación local y ejecución

4/4 comprobaciones locales: siete SELECT acotados sin efectos/bloqueos; cambio de revisión/autoridad/inventario/actividad bloquea; salida cerrada rechaza contenido privado; host compilado con la DLL real omite un texto privado en un fallo de lector. Compilación del auxiliar correcta, sin sustituir binarios IIS.

Run9f11f7ba36074e23b35efec9f65de16e finalizó PURE_PREFLIGHT_PASSED:7 SELECT,0 HTTP/sesiones/efectos,1.000 archivos/1 raíz/11 intentos,1.000 pruebas y727.503 caracteres de controles. DLL b3303603a03001e2a1f578d48ed4a3f6ea396c0b0faa162544941c7f9485e12f; cerrado y credenciales eliminadas. El cálculo puro no reprodujo el500 y no acredita la construcción de la vista focal del repositorio.

## Lectura nativa adicional y límites del auxiliar

Se preparó un modo con el proveedor MySQL de la DLL: máximo12 SELECT reservados por método, sin transacciones/FOR UPDATE ni instancia del repositorio. Lee contexto, cinco catálogos de metadatos, índice, lote/control y plan. Reporta un límite superior reservado; no inventa el conteo exacto dentro de una llamada privada.

Run59c48d91 cerró en NATIVE_INPUT,0 SELECT: el compilador produjo un ejecutable32 bits mientras el DSN está en Registry64. Fue un defecto del auxiliar, no una reproducción del500. Se fijó /platform:x64 y la comprobación local verifica PE32+ antes de usarlo. Runad60fe52 cerró en NATIVE_LOT, límite superior9 SELECT: metadatos e índice retornaron, después un guard del propio auxiliar rechazó el lote. Su InvalidOperationException sin método del ensamblado no acredita la excepción de la aplicación. Se eliminó la condición incorrecta que exigía ausencia de un control ya existente; no se relanzó este modo para atribuir otra causa. Ambos runs cerrados y secretos eliminados,0 HTTP/sesiones/efectos.

## Defecto reproducido en la DLL instalada

`VistaBarreraCreacionImportacion.Raices` y `.Archivos` no tienen inicializador por defecto. `CargarBarreraFocal` construía esa vista y ejecutaba inmediatamente `vista.Raices.Add(raiz)`. La verificación del constructor real faltaba en los puertos de las pruebas anteriores.

Reproducción local invoca ese método privado del binario instalado con una raíz pendiente sintética y conexión nula: **NullReferenceException, TargetSite=CargarBarreraFocal**, sin consultas ni escritor. Sobre la candidata corregida, la misma llamada llega a la comprobación del proveedor por conexión ausente (**InvalidOperationException, TargetSite=CheckState**); es el fallo intencional posterior de la reproducción, no un error de importación real.

Corrección localizada: `CrearVistaBarreraFocal`, utilizado por el repositorio, inicia Raices con la raíz objetivo y Archivos con una lista vacía. Conserva las validaciones anteriores y la carga del candidato pendiente; no modifica el constructor global del DTO, esquema, reservas o escritor. La regresión verifica el constructor real para raíz pendiente y creada, identidad/contexto conservados, lista inicial y agregado de candidato.

Verificación:20/20 Node (16 independientes+4 del auxiliar),2/2 wrappers Framework/DDL con el nuevo caso del repositorio, compilación completa aislada0 errores y flujo actualizado124 fuentes/20.075 líneas. No se suman casos internos a los wrappers. La corrección local completa3.199/3.200 con estos límites;3.198 sigue pendiente de los tres guardados reales corregidos. Candidata DLL8b27a942458d41aa87d8ddd0df684bf70b7b89ec7702f0607958169a91cc571d preparada con respaldo; no instalada al registrar este corte.
