# Lote50: corrección instalada y tres guardados confirmados

La DLL/PDB del commit8f71ec7e corrige las listas no inicializadas de la barrera focal. Instalación aprobada con respaldo verificado,2026-11-18T15:50:12Z. DLL `8b27a942458d41aa87d8ddd0df684bf70b7b89ec7702f0607958169a91cc571d`; las fuentes y regresión se detallan en `doc120-preflight-puro-20261118.md`. El escritor compartido, reservas, autoridad, esquema y procesamiento por archivo conservan su recorrido.

Nueva ventana aprobada para tres pendientes: runc6f62c443bf64e039878754736ceb1b4, reutiliza el ejecutor existente y revisa la corrida42318523 cerrada. No modifica sus resultados, ni relanza su proceso. Ambiente localhost autorizado, propietario141/empresa2/nivel223, lote50/plan90/versión45.

| Archivo | Documento confirmado | Inventario | Bytes originales | HTTP de ALMACENAR |
|---|---|---|---|---|
|107|16267|23396|44.885|1,352934s|
|108|16268|23397|44.187|0,913335s|
|109|16269|23398|44.688|0,908159s|

Los tres documentos se comprobaron en RA y DA, gabinete10 y expediente693. El lote pasó de10 a13 archivos completados; la huella de los otros997 recursos permanece idéntica. Comprobación posterior de cada checkpoint y de los documentos ya confirmados; sin repetir104/105/106. Consumo104 HTTP/20 SELECT externos/1 sesión/3 solicitudes ALMACENAR. FINISHED_THREE, sin fallo, cierre15:55:19Z y credenciales eliminadas. La aplicación no preparó lotes, recibió originales, creó expedientes, reanudó ni borró durante esta ventana.

## Comparación y límites

Promedio HTTP observado: **1,058143s por archivo**, frente79,432043s de la referencia23e18a40 (tres archivos104/105/106 de tamaños44.683/44.884/44.884 bytes). Es una comparación de dos muestras de tres archivos distintos del mismo lote; no una medición pareada ni certificación de1.000 guardados. El control focal ya existía antes de esta corrida:1.000 pruebas y1 cabecera antes/después. No se vuelve a generar; solo crecen las referencias de documento/inventario de10 a13. El costo inicial del sellado no está incluido.

Cada solicitud registró11 lecturas instrumentadas de archivo,35 de evento y120 de esquema. La lectura de esquema todavía consume aproximadamente0,55–0,58s por solicitud. Es un costo observable para un análisis futuro; no se elimina autoridad ni se añade otra optimización en esta corrección. Los SELECT externos de la prueba se distinguen de los internos instrumentados; no se suman etapas anidadas ni se cuentan como instrumentadas las lecturas propias del escritor compartido. El JSON/CSV compañeros conservan las categorías y conteos originales.

3.198 completa en su alcance de tres archivos.3.187/3.196 siguen pendientes: esta corrida no ejecuta el caso A/B ausente/C, carreras o rollback reales MySQL. La aceptación integral y capacidad de1.000 siguen condicionadas a las brechas previas. No repetir automáticamente esta ventana cerrada.
