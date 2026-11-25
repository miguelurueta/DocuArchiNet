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
   `edd033a113b57950cac1661b1a5cc01ff114fc8ea0676082c0f9e590aa3e957a`.
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

## Ampliación del asistente — pendiente

El corte local actual es 47/52 y 160 pruebas. Ver doc126-asistente-20261124.md.
La candidata anterior se sustituye por el hash indicado arriba; preparar scope con
DLL/assets vigentes. La matriz de cinco casos no cubre 8.8 y su scope bloquea
GuardarPlantilla. Antes de ampliar la E2E se debe autorizar específicamente el alta
de una plantilla y el destino nuevo, sus efectos máximos y conservación. No
reutilizar un fixture ya creado ni retirar el bloqueo sin un scope revisado.
El asistente no requiere otro SQL de instalación; cualquier fixture/adecuación
adicional que resulte necesario se entrega para aplicación manual separada.

## Corte vigente: desplegado e instrumentado

Ver [evidencia actual](doc126-despliegue-e2e-preparada-20261124.md). La DLL candidata ya se desplegó con respaldo y comprobaciones sin autenticar; el paso de publicación anterior queda acreditado en ese alcance. Queda pendiente la aceptación con cuentas.

El modo --excel-ui-assistant está implementado con autorización propia, una alta exacta de plantilla/dos campos, destino nuevo, 1000 filas y cero borrados. No necesita SQL23–25, sí instalación compatible, conexión/cuentas verificadas y autorización específica. La matriz anterior de cinco casos continúa esperando confirmación de esos SQL. Las dos ventanas juntas permitirían como máximo 4311 filas de entrada (incluidas rechazadas/externa), 1002 borrados intentados y una plantilla de dos campos. No ejecutar por esta documentación: los alcances locales siguen incompletos y no autorizados.

## Corte vigente: diagnóstico v4 publicado

DLL y assets actuales en doc126-diagnostico-configuracion-20261124.md/JSON. El scope exacto de alta requiere No/No/No y posición0 en campos nuevos, preserva históricos y conserva límites/autoridad. Refrescar hashes y preflight antes de cualquier ventana. SQL23–25 siguen pendientes de confirmación para la matriz previa; no se requiere otro SQL para este diagnóstico. 52/57 tareas, cinco de aceptación/gobierno pendientes.

## Recorrido funcional con cuenta existente — 2026-11-24

57/62 tareas: completada la adaptación 6.12; pendientes 6.9, 6.10, 6.11, 7.5 y 8.8. Usuario 141 activo y autorizado con sus cinco permisos vigentes, conservados. Modo funcional de agregar/reemplazar separado de exclusividad/restricción; 88/88 pruebas locales y cuatro E2E registradas, sin ejecutar la nueva ventana real. Preflight SELECT: faltan tablas, políticas y plantillas DOC126. Copias SQL23–25 parametrizadas en .tmp/doc126-functional-preparacion, sin aplicar. Ver `Doc/Actualizacion/Importacion/Validacion/doc126-cuenta-existente-funcional-20261124.md`.

## SQL23–25 aplicados con autorización — 2026-11-24

El usuario autorizó aplicar las copias parametrizadas. Dos tablas, dos políticas y tres plantillas activas verificadas desde conexiones nuevas; IDs57/58/59, filas sintéticas iniciales1/2 y destino CREAR ausente. Las indicaciones PENDIENTE de cortes anteriores son históricas: no reaplicar. La E2E funcional usa la cuenta141 con permisos actuales conservados y plantillas58/59; scope vigente válido y27 tablas protegidas. Consola privada preparada; no se acredita ejecución hasta disponer de su resultado. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-aplicacion-sql-funcional-20261124.md`.

## E2E funcional detenida en autenticación — 2026-11-24

SQL23–25 aplicados y verificados; no reaplicar. Corregido control compartido para la columna histórica AÑO_FERIADO, con38/38 regresiones. Segundo intento completó27 huellas iniciales, pero el servidor devolvió LOGIN_CREDENTIALS_REJECTED en el módulo GESTOR del perfil local. Cero sesiones autenticadas/cargas nuevas; fixtures permanecen1/2 filas. Confirmar el acceso manual y proporcionar la contraseña sólo en consola privada. No se cierran tareas de aceptación por estos intentos. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-aplicacion-sql-funcional-20261124.md`.

## Aceptación funcional real con la cuenta existente — 2026-11-24

E2E Playwright real PASS: un test, dos casos. Agregar1000:1→1001 filas; reemplazar1000:2→1000 filas, con consentimiento. Usuario141 en IIS local HTTPS/GESTOR, plantillas58/59 y hashes/propiedad/locks verificados. Huellas íntegras de27 tablas protegidas y configuración de plantillas sin cambios. Perfil local separado; perfil compartido, permisos, DLL y Web.config conservados. La creación/importación manual del usuario se reconoce como realizada. No acredita exclusividad/restricción, rollback/carrera o asistente automatizado. Ver `Doc/Actualizacion/Importacion/Validacion/doc126-e2e-funcional-real-20261124.md`.

## Rollback y concurrencia reales — 2026-11-24

6.11 completada:58/62 tareas. E2E real PASS, dos casos: rollback conserva íntegramente1000 filas; concurrencia conserva las1001 anteriores y sólo una fila externa adicional (total1002), con cero filas propias confirmadas. Actor observó INSERT bloqueado y locks activos antes de confirmar.27 tablas protegidas, configuración y dos resultados positivos previos intactos; locks liberados.41/41 pruebas Node y4607 checks sintéticos. Sin instalación/limpieza/permisos nuevos. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-e2e-rollback-concurrencia-20261124.md`.

## Asistente y tres modos reales — 2026-11-25

6.10 y8.8 completadas:60/62 tareas. E2E real del asistente PASS: una plantilla nueva/dos campos, CREAR1000 filas y29 tablas protegidas con huellas intactas. Combinada con AGREGAR/REEMPLAZAR anteriores acredita1000 filas por cada modo; rollback/concurrencia ya aprobados. Se conservan permisos, históricos y cuatro resultados previos; sin SQL de instalación, limpieza o cambio productivo.43/43 regresiones de adaptadores PASS. Pendientes6.9 (cuentas exclusiva/restringida) y7.5 (cierre de gobierno). Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-e2e-asistente-real-20261125.md`.
