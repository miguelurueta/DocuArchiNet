# DOC-116 — referencias del ambiente por lectura

Diagnóstico real terminado con PASS: cuatro consultas SELECT, cero escrituras, creaciones o cambios de permisos. Se reutilizó autorización existente del ambiente/cuenta y el DSN MembershipUsers con cuenta exclusivamente de lectura; credenciales efímeras introducidas solo en consola privada. No se abrió una sesión de aplicación ni se ejecutó aceptación E2E.

Actor141/empresa2 confirmado activo. Se encontraron 91 niveles candidatos por propiedad/concesión. Niveles7/8 son propiedad del actor; nivel215 es no propietario y tiene concesión629 de agregar_expediente y carga_archivo. Son referencias para preflight, no autorización para usarlas o cambiarlas. Las cuatro tablas examinadas son InnoDB.

Se revisaron once lotes. Ninguno tiene dos raíces nuevas y dos archivos recibidos sin efectos previos. Lote10 conserva una recepción y lote11 ninguna; no se reciben archivos ni se crean expedientes sobre ellos. Los lotes históricos se conservan intactos.

Para EXP01 se propone un setup nuevo acotado seguido de la creación: un plan, un lote, dos PDFs de 677 bytes, dos expedientes bajo nivel7, cambio visible a nivel8 y cuatro solicitudes de creación contando replays. Cero documentos definitivos, cambios de permisos/perfiles o almacenamiento legacy. Nombres DOC116-20261105-EXP01-01 y DOC116-20261105-EXP01-02. Perfil24/publicación38 se revalida por servidor; no se publica otro perfil. Propuesta exacta en doc116-exp01-recursos-propuestos-20261105.json. No está autorizada ni ejecutada.

EXP03 dispone de una referencia no propietaria y motores compatibles, pero todavía necesita lote/nombres revisados y autorización expresa de revocación/restauración. EXP02 interno conserva su dependencia técnica; un resultado descartado en cliente no lo acredita.
