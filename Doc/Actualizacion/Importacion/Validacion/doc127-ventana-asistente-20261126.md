# DOC127: ventana del asistente XLS y XLSX preparada

**Estado actual: EJECUTADA2/2 PASS; AUTORIZACIÓN CONSUMIDA.** [Resultado real](doc127-asistente-real-20261126.md);5.2 y7.1 completadas,31/33 tareas. La descripción inferior conserva el corte histórico de preparación.

**Estado al preparar: PREPARADA, PENDIENTE DE AUTORIZACIÓN.** No hubo autenticación ni importación. Avance **29/33 tareas**;5.2 continúa abierta. La capacidad conserva18/18 casos reales aprobados.

Lote `c659ec0b64d94681bccaaa8e6e75e4d0`, SHA de preparación `1e5a19943665e55dd6b05707609359194d1001f3`. Dos recorridos secuenciales del asistente actual: XLS y XLSX. La evidencia XLSX de DOC126 se conserva como histórica; la interfaz cambió posteriormente en asociación de formularios y manejo de foco/busy. [Matriz de plantillas](doc127-matriz-plantillas-20261126.md).

## Ambiente y efectos propuestos

IIS local `https://localhost/GestionDocumental-Docuarchi.net/`, módulo GESTOR, empresa2, DA11/RA12, conexión6 y cuenta194 (asesor.comercial). Activa y con sólo cargas habilitadas; no se cambian sus permisos. Una contraseña efímera, capturada en consola privada después de autorizar. No se autentican196 ni141.

| Formato | Destino nuevo | Archivo | Campos propuestos |
| --- | --- | --- | --- |
| XLS | imp_02_doc126_e2e_asistente_fab26227 | 229376 bytes;1000 filas | CLAVE VARCHAR(61); VALOR VARCHAR(63) |
| XLSX | imp_02_doc126_e2e_asistente_3d2f2ad4 | 16301 bytes;1000 filas | CLAVE VARCHAR(62); VALOR VARCHAR(65) |

Cada archivo usa hoja Datos, clave CLAVE y valores sintéticos nuevos. El asistente debe analizar, mostrar la propuesta, guardar explícitamente una plantilla/dos campos, conservar el archivo, recibirlo, prepararlo, confirmar una sola vez y consultar el resultado por ID. Revisión esperada1 y estado CONFIRMADA con1000 filas. Guardar la plantilla no puede importar filas.

**Máximo total: dos plantillas nuevas, cuatro campos, dos tablas y2000 filas nuevas. Cero borrados y cero cambios en datos existentes o permisos.** Conservar todos los resultados. No instalar SQL, sembrar datos fuera del flujo funcional, limpiar, desplegar o cambiar operaciones a AGREGAR/REEMPLAZAR.

## Conservación, recuperación y presupuesto

Proteger las38 tablas actuales, configuración,78 perfiles y41 cargas. El primer caso compara38 tablas; el segundo añade a sus protegidas la tabla XLS recién creada y compara39. Controles SELECT antes/después conservan todas las cabeceras/campos/resultados anteriores; sólo excluyen las dos altas concretas, que se verifican separadamente por IDs, propietario, conexión, hash del archivo, campos y conteo.

Un lote, una ejecución por formato y cero reintentos. El segundo caso exige PASS real y control posterior del primero. Timeout de15 minutos por test y vigencia máxima de45 minutos desde la autorización del lote. Ante fallo o incertidumbre, detener el lote, recuperar IDs/estados por SELECT y conservar todo; no volver a recibir, preparar o confirmar automáticamente.

Se reutilizan runner DOC110, --excel-ui-assistant, suite Excel, soporte DOC126 y controles DOC32. Scopes y consola están en .tmp/doc127-assistant-ventana/ y .tmp/doc127-assistant-session.ps1; son datos locales excluidos de Git. Las guardas se ejecutan antes de credenciales y validan autorización, vigencia, hashes y consumo único. La consola termina naturalmente y guarda sólo salida saneada después de capturar la contraseña.

## Verificación ya realizada

Dos análisis nativos locales con el servicio/lector del producto: formatos, hash, hoja,1000 filas y dos tipos de campo coincidentes.14 SELECT comprobaron identidad/permisos, ambiente físico, referencias, destinos y archivos sin uso previo y huellas de conservación. Nueve comprobaciones locales de guardas PASS, cinco scripts Node con sintaxis válida y dos scripts PowerShell sin errores de análisis. Ninguna autorización o reclamación real fue creada por las pruebas: las variantes autorizadas se simularon únicamente en memoria.

Los ocho hashes de fuentes conservan las55 pruebas locales anteriores;17 hashes conservan la matriz de capacidad. No se anuncia una nueva ejecución de esas pruebas ni del asistente. La preparación no completa5.2 ni aporta QA humana nueva.

## Autorización pendiente

La autorización anterior de autoridad/lectura está consumida. AGENTS.md exige: «No ejecutar E2E real ni carga sin autorización explícita para el ambiente y las cuentas de prueba». Esta nueva ventana necesita aprobación para la cuenta194 y los efectos enumerados. No abrir la consola ni activar las banderas de autorización antes de recibirla.

EXPLAIN29 sigue reservado a ejecución manual: no se encontró un plan del servidor aportado. Siguen pendientes2.4,5.2,7.1 y7.5. El JSON adjunto fija recursos, hashes, presupuestos, resultados locales y límites.
