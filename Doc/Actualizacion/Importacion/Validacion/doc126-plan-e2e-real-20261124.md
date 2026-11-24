# DOC126: ventana real preparada, sin ejecutar

Implementación local: 40/44 y150/150 pruebas. La ventana necesita confirmación específica;
los resultados/credenciales/autorizaciones DOC125 no se reabren ni se heredan.
La regla aplicable está en [AGENTS.md](../../../../AGENTS.md): «No ejecutar E2E real
ni carga sin autorización explícita para el ambiente y las cuentas de prueba».

1. Revisar [SQL 22](../Sql/22-preflight-doc126.sql) en sus bloques RA/DA y confirmar
   las cuentas de la empresa autorizada. Propietario: cargas=1 y los cuatro otros
   permisos de importación en 0. Restringido: activo y cargas=0. No modificar
   cuentas/permisos para forzar el resultado; una adecuación requiere otro plan.
2. Aplicar manualmente [SQL 23 DA](../Sql/23-fixtures-da-doc126.sql),
   [SQL 24 RA](../Sql/24-fuentes-e2e-ra-doc126.sql) y
   [SQL 25 DA](../Sql/25-plantillas-e2e-da-doc126.sql), con las precondiciones y
   COMMIT/ROLLBACK indicados en [la guía](../Sql/README.md). Confirmar aplicación
   y conservar los IDs resultantes. No reaplicar 18–21 ni tocar sus datos.
3. Autorizar despliegue local de la DLL candidata con respaldo y comprobación
   WSDL, conservando Web.config/recursos actuales. Candidata:
   `525ab00555e7be727e4f37bbf316a5b7adaee40a4c683c55e422f4bdeb50d257`.
   Los tres assets, ASPX y namespace contextual pertenecen a la misma entrega.
   Preparar scope con sus hashes exactos y las referencias verificadas, sin claves.
4. Autorizar lectura real con las dos cuentas y recursos existentes mediante
   `test:doc110:session -- --excel-ui-read`, sin edición/recepción/confirmación.
5. Autorizar escritura real mediante `--excel-ui-write`: CREAR: 1000, AGREGAR: 1000,
   REEMPLAZAR: 1000, rollback: 300 y carrera: 10 con una fila externa. Máximos previstos:
   3311 filas de entrada incluidas las rechazadas/externa y 1002 eliminaciones de
   fixture intentadas (incluye el rollback). Efectos sólo en `imp_02_doc126_e2e_*`
   y sus nuevas ejecuciones; propietario/conexión/plantilla/archivos/hashes fijados.

Los cinco XLSX sintéticos ya se generaron y leyeron íntegramente con el generador
DOC125 y lector DOC123 existentes. Rutas y hashes quedan en el manifiesto local
excluido de Git `.tmp/doc126-fixture-plan.json`. Completar el scope después de
verificar los SQL; valores incompletos detienen el runner antes de autenticar.
Los DSN DA/RA y credenciales se resuelven con recursos actuales y valores efímeros;
no se añade conexión a Web.config. Solicitar las claves sólo en la consola privada
existente al abrir la ventana autorizada, sin imprimirlas ni guardarlas.

La E2E usa controles visibles, no sustituye el recorrido por llamadas directas.
El acuse ignorado es una decisión del cliente después de una respuesta del servidor;
no acredita un corte físico de COMMIT/red. La carrera debe observar el INSERT del
dueño del lock y acreditar commit externo/rollback propio. Preservar CONTRATOS,
EGRESOS, CRÉDITOS, configuración anterior y todos los resultados DOC125/nuevos,
con huellas completas SELECT. No instalar, sembrar, reparar ni limpiar desde tests.

Hasta confirmar esos requisitos,6.9/6.10/6.11 y 7.5 permanecen abiertas; Jira y archivo
OpenSpec no se cierran. Esta preparación no es evidencia de una ejecución real.
