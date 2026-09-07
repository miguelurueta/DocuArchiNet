# Diseño técnico — DOC-52

## Contexto

DOC-50 define el contrato multiproveedor y DOC-51 la frontera HTTP. DOC-52 prepara una importación sin efectos y persiste su identidad para que entregas posteriores puedan ejecutar/reconciliar de forma segura.

## Decisiones

### D-01 — Contratos canónicos extendidos
DTO, modelos e interfaces existentes incorporan snapshot autoritativo, resultado preflight, solicitud de creación, intención y elementos. No se duplican contratos en Services o Infrastructure.

### D-02 — Preflight puro
`ServicioPreflightImportacion` reutiliza el validador de contexto y valida selección no vacía, `clientItemId` único, identidad externa, tipología/destino y capacidades. Produce requisitos y comandos tipados sin repositorio, SQL, Session o almacenamiento.

### D-03 — Identidad explícita
La identidad portable es la pareja normalizada `providerId + externalKey`. Cada elemento incluye `taskId` destino; ningún dato general se toma implícitamente del primero. El adaptador SII futuro resolverá su identificador oficial.

### D-04 — Huella canónica
`ServicioIntencionImportacion` genera SHA-256 sobre schema, operación, usuario/grupo, tarea/ruta/trámite, proveedor, requisitos y elementos ordenados por identidad. Se usan codificación UTF-8, cultura invariante, longitudes delimitadas y normalización definida para evitar ambigüedad.

### D-05 — Persistencia transaccional idempotente
El repositorio abre conexión con `ModuleConnectionFactory`, inicia transacción, intenta reservar la clave única, lee la fila vencedora ante duplicado y compara huella. Igual huella retorna la intención con `Reused=True`; distinta retorna `IDEMPOTENCY_CONFLICT`. Cabecera, requisitos y elementos se guardan en la misma transacción.

### D-06 — Concurrencia en dos capas
`MySqlImportIntentConcurrencyGuard` ofrece exclusión cooperativa acotada por hash de contexto/idempotency key. La restricción única en MySQL sigue siendo la autoridad, incluyendo procesos distintos o caída del lease.

### D-07 — Esquema moderno y SQL parametrizado
Tres tablas nuevas almacenan intención, requisito y elemento; claves foráneas eliminan hijos en rollback lógico de creación fallida. Todos los valores funcionales usan `IDataParameter`. El DDL vive en `Sql/`, se aplica manualmente y tiene rollback separado; no altera tablas legacy.

### D-08 — Estados y auditoría
Una creación exitosa queda `Creada`; cada elemento también inicia `Creada`. Se almacenan `operationId`, `correlationId`, contexto original, versión y fechas UTC. La auditoría técnica no almacena contenido, credenciales o respuestas externas.

### D-09 — Validación aislada
Pruebas CommonJS verifican estructura, contratos, SQL parametrizado, huella, reutilización y colisión mediante dobles. No hay endpoint/UI: E2E Playwright no aplica. No se conecta MySQL ni se ejecuta DDL.

## Flujo

1. Preflight revalida contexto y selección y devuelve plan sin efectos.
2. Creación construye snapshot y huella canónica.
3. Guard limita concurrencia cooperativa.
4. Repositorio reserva idempotencia y persiste agregado en una transacción.
5. Colisión equivalente reutiliza; colisión distinta falla explícitamente.

## Riesgos

- Identidad SII aún no confirmada: se mantiene opaca como `externalKey` y no se inventa semántica.
- DDL no aplicado: código queda inactivo hasta despliegue manual aprobado.
- Atomicidad solo local: no se prometen efectos externos/documentales en DOC-52.

## Reversión

El código es aditivo. El rollback operativo evita conectar consumidores; el rollback SQL elimina primero elementos/requisitos y luego intenciones, únicamente con autorización y tras verificar ausencia de datos necesarios.
