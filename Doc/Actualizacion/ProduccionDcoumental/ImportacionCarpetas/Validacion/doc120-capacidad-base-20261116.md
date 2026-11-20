# DOC-120 — Línea base local de capacidad y contrato propuesto

Fecha2026-11-16. Base875403f550a72820f9841f38f93991ecde9fbfe5, rama feature/DOC-120, carpeta principal. Resultado: **PASS_LOCAL_DIAGNOSTIC; capacidad operativa no aceptada**. Tarea3.98 completada;3.97 tiene contrato concreto pendiente de aprobación. [Resultado estructurado](doc120-capacidad-base-20261116.json), [contrato propuesto](../Diseno/08-preparacion-segmentada-capacidad.md).

## Reproducción

El auxiliar `tools/e2e/scripts/measure-doc120-client-capacity.cjs` importa InventarioImportacion, ImportacionCarpetasApi y el motor crearCoordinacion reales. Solo sustituye transporte, consulta y efectos por dobles locales. Reutiliza el modelo de fases y el coste por enumeración del binding actual, sin introducir otra máquina de pruebas.

```powershell
node tools/e2e/scripts/measure-doc120-client-capacity.cjs
node tools/e2e/scripts/measure-doc120-client-capacity.cjs --plan .tmp/doc120-1000-local-plan.json
```

La primera ejecución usa metadatos sintéticos y no requiere archivos externos. La segunda lee el plan excluido de Git del corpus ya preparado y hace stat de sus1.000 archivos; no lee su contenido PDF ni cambia archivos. El JSON de evidencia guarda únicamente cantidades/tamaños, versiones, tiempos/heap muestreados y huellas de fuentes, sin rutas absolutas del corpus ni credenciales.

## Resultado observado

Inventario real del corpus: una raíz,1.000 archivos,44.859.102 bytes. El cuerpo de PrepararPlan tiene172.409 caracteres UTF-16 y172.409 bytes UTF-8 para estos nombres. Con el límite ASMX de referencia102.400, el preflight existente devuelve REVISION_TAMANO_EXCEDIDO antes de crear XHR. No se inspeccionó IIS; la medición no afirma cuál fue su límite/respuesta durante el video.

| Escenario local | Lecturas por snapshot modeladas | Efectos sintéticos realizados | Lecturas simuladas aceptadas | Resultado del motor |
| --- | --- | --- | --- | --- |
| Una raíz/1 archivo |4 |4 |20 | Completo |
| Una raíz/100 archivos |7 |301 |2.114 | Completo |
| Una raíz/1.000 archivos |43 |232 |10.000 | Bloqueado: COORDINACION_LIMITE |

En el último caso hubo232 recepciones sintéticas acusadas; la última lectura global completa aún mostraba231. El efecto232 quedó acusado y el intento de lectura siguiente agotó el presupuesto; no se repitió ni se solicitó otra mutación. No se alcanzaron creación, valores o almacenamiento en ese escenario. La distinción entre acuse y última proyección evita atribuir una lectura fallida a una recepción perdida.

El coste de terminar ese recorrido con el binding actual es derivado:3.001 efectos y43×(3.001+1)=129.086 lecturas. No se ejecutaron129.086 solicitudes, no se midió el rendimiento de BD/IIS y no se importaron documentos reales. El auxiliar reproduce el coste del binding al consumir el presupuesto del motor; no ejecuta su transporte readAll. El JSON conserva CPU/heap muestreados de Node, que no representan un pico exacto ni memoria del servidor/navegador.

## Presupuesto de implementación propuesto

El contrato08 fija tamaño real por operación: hasta100 entradas y65.536 caracteres de petición, reducidos por el límite efectivo; respuestas acotadas a65.536 y fragmentos canónicos a262.144. El resultado de reglas/incidencias puede ocupar varios fragmentos aunque provenga de una sola parte de entrada. Nunca se eleva el límite de perfiles ni se serializa el plan completo para comprobarlo.

Para la coordinación: enumeración inicial de43 lecturas y hasta dos lecturas focales por efecto, <=6.045 consultas para el recorrido ordinario de1.000 archivos/una raíz. Objetivos locales: recorrido simulado completo <=10s en Node, crecimiento de heap muestreado <=128MiB; Chromium <=2.000 nodos dentro del asistente, actualización de avance p95<=100ms y crecimiento de heap <=128MiB. Registrar versión/máquina/método y corregir o acordar explícitamente cualquier incumplimiento. Estos son objetivos propuestos de diseño, todavía no resultados del cliente optimizado.

Los límites HTTP/SELECT/disco/tiempo/memoria operativos se definirán en3.125 y autorizarán en5.2. Medir menos HTTP no acredita automáticamente menor trabajo SQL; el lector focal debe medir también consultas/filas y coherencia de revisiones.

## Alcance y comprobaciones

- Tres escenarios del motor con aserciones de estados/conteos y repetición con metadatos sintéticos; no se suman como una nueva suite integral.
- Seis huellas SHA-256 normalizadas a LF verificadas, incluido el auxiliar reproducible. Sintaxis Node y OpenSpec estricto comprobados en este corte.
- Cero HTTP de aplicación, sesiones autenticadas, SELECT reales, bytes PDF recibidos, expedientes/documentos creados, DDL o activación IIS.
- La propuesta del contrato08 especifica siete operaciones ASMX, tres entidades aditivas, esquema de plan2 y reversión compatible. No modifica producto ni se declara aprobada por existir este documento.
- La capacidad real, privacidad/recuperación autenticadas y aceptación integral siguen pendientes; las evidencias históricas permanecen intactas.
