# DOC-115 — 07-A, integración MySQL independiente

Estado: EJECUTADO_OK, intento 3; finalizó 2026-11-04T13:40:08.617Z. Consola, diario, autorización y fuentes coinciden. La revisión no repitió pruebas reales ni abrió otra conexión. Los intentos 1/2 permanecen como fallos de preflight, sin escrituras.

Planes 29/30/31 y lotes 10/11; archivo operativo 16 e intento de recepción 30. Son entidades de importación, no expedientes ni documentos definitivos. Se conserva un PDF original de 677 bytes en staging y todos los recursos. Se verificaron tres planes, dos lotes, cuatro raíces, cuatro archivos, un intento y cuatro eventos nuevos confirmados; las transacciones revertidas no se cuentan como recursos confirmados.

- Dos confirmaciones del mismo plan convergieron al lote 10; el replay conservó identidad y otro payload se rechazó.
- Dos planes diferentes compitieron por otra clave: un lote confirmado y un conflicto. El plan perdedor permaneció revisable.
- Pasaron los tres rollbacks de confirmación: ANTES_EVENTO, EVENTO_INSERTADO y PLAN_CONFIRMADO. Se verificó además la repetición del lote previamente confirmado.
- Dos reservas concurrentes conservaron un intento y una reclamación; una revisión obsoleta con otra solicitud se rechazó.
- El cuarto fallo controlado demostró rollback de la recepción provisional sin borrar el completo de disco. Dos confirmaciones de recepción conservaron una sola revisión positiva, un intento y un evento de recepción.

Las llamadas usan conexiones físicas nuevas, con Pooling=false exclusivamente en la cadena efímera del test. Se contrastaron por SELECT las trece huellas de protección anteriores/posteriores: seis entidades operativas existentes, perfiles, versiones, reglas, permisos y conteos de expedientes, documentos e inventarios. Todas coinciden. Los controles externos usaron la cuenta de lectura; los SELECT de observación provisional pertenecen a la transacción comprobada. No hubo DDL ni cambios de configuración/permisos/perfiles/backend.

Código verificado: b84f54bc8081811edd9488fa89ce3380e412faaa065b6d62d9ad943dc729c65b (lista de diez fuentes en el JSON de evidencia). Backend: 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5. La autorización también vincula la huella del archivo de recursos. El contexto sintético se limita a repositorios: no sustituye autoridad HTTP ni preflight de 06, ya cubiertos por RECIB. Huecos AUTO_INCREMENT de conflictos/rollback son esperables; no se exige secuencia sin huecos.

Tareas 2.1/2.2/2.3/2.4/5.1 completadas. 07-A y los tres RECIB cuentan con evidencia real; el cambio llega a 28/28 tareas. Publicación y cierre Jira aún no realizados.
