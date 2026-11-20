# DOC-120 — rendimiento real ya medido

El usuario confirma que el rendimiento ya se determinó dentro de las pruebas y solicita revisión focal. Se revisaron los resultados y se recalcularon las estadísticas desde los cuatro CSV existentes. **No falta ejecutar otra medición para consolidar el rendimiento observado.** Se cierra 3.126 como consolidación de resultados existentes y 5.2 como registro resuelto de autorizaciones, sin solicitar una nueva ventana. Avance **266/270**, cuatro tareas abiertas.

## Mediciones reales y procedencia

| Corte y fuente | Guardados | Media HTTP por guardado | Rango | Resultado |
| --- | ---: | ---: | --- | --- |
| [Diagnóstico inicial](doc120-medicion-tres-20261117.md), CSV compañero | 3 | 79,432043 s | 70,899768–93,176877 s | Identificó coste de metadatos/pasadas completas; run23e18a40. |
| [Corrección medida](doc120-lote50-correccion-medida-20261118.md), CSV compañero | 3 | 1,058143 s | 0,908159–1,352934 s | Guardados confirmados RA/DA y997 recursos ajenos preservados; runc6f62c44. |
| [Diez siguientes](doc120-lote50-diez-20261118.md), CSV compañero | 10 | **0,969465 s** | 0,897519–1,225048 s | FINISHED_TEN,990 ajenos preservados; run3d30f47d. Mediana0,934637 s y p95 muestral1,140969 s. |
| [Guardado real con pausa/cierre/retoma](doc120-guardado-real-resultado-20261119.md), CSV compañero | 4 | **0,980750 s** | 0,938000–1,022000 s | PASS_REAL_STORAGE_UI; run08bc33cc. Medición antes de retener la entrega al navegador. |

Las medias, mínimos, máximos y p95 se recalcularon desde elapsedMs, usando cultura numérica invariante. El p95 usa interpolación lineal en posición(n−1)×0,95. Coinciden con los primarios publicados; las pequeñas diferencias de precisión respecto del JSON original proceden del redondeo del CSV, no de una corrida nueva.

La DLL de la medición de cuatro coincide por SHA-256 con la instalada al revisar: b0b5d0d329f62124aa45ea49248746b90ad40c824a8a0a6adf73291d5d39dc4f. Se verificó únicamente su archivo local, sin consultar la aplicación ni desplegar otro binario.

La muestra de diez no muestra crecimiento progresivo: primera mitad1,014579 s y segunda0,924352 s. El diagnóstico inicial atribuye72,321% del coordinador a metadatos y0,082% al escritor habitual; son categorías anidadas, no porcentajes que puedan sumarse indiscriminadamente. Los cortes se conservan separados por versión, archivos y objetivo; no se presentan como una muestra única o comparación pareada.

## Ejecución completa y límites conservados

El [control final del lote52](doc120-lote52-control-final-resultado-20261120.md) acredita por separado1.000 recibidos/guardados,44.859.102 bytes, un expediente y1.000 documentos/inventarios distintos, sin intentos pendientes o duplicados de identidad. Junto a las mediciones reales anteriores y la confirmación actual del usuario, resuelve el pendiente de consolidación; no se exige volver a importar ese corpus.

Estos datos determinan la latencia observada de almacenamiento en los casos ejecutados. No se multiplica0,969465 por1.000 para inventar la duración completa: los HTTP medidos excluyen login, interfaz, controles externos, recepción y otras fases. Tampoco se convierten muestras pequeñas en garantías para PDF grandes, sellado en frío, memoria máxima de IIS/navegador, crecimiento total de disco o capacidad máxima. Los límites de cobertura quedan en la revisión final6.4, sin introducir una prueba adicional como pendiente automático.

## Autorizaciones y tareas

- **3.126 completa:** consolida la importación funcional ya finalizada y las mediciones reales autorizadas de tres, diez y cuatro guardados. No se declara una medición integral que no exista.
- **5.2 resuelta:** las corridas utilizadas tienen sus autorizaciones y ventanas propias en sus fuentes. No hay otra ejecución necesaria para esta consolidación; la casilla no debe generar una solicitud de autorización circular. Esto no autoriza retroactivamente el plan propuesto3.125 ni concede futuros efectos de otra ventana.
- **6.4 permanece abierta:** aceptación final de resultados y límites con su clasificación efectiva, incluidos los alcances muestrales. Si posteriormente se solicita una prueba diferente, se delimita y autoriza antes de ejecutarla conforme a D-08/RQ-08.

Revisión documental y aritmética únicamente. Cero nuevas pruebas E2E, HTTP de aplicación, conexiones, SELECT de control, cambios de datos, DDL, DLL o producto. Las ventanas originales permanecen cerradas y sus resultados intactos.
