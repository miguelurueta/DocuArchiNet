# Procesamiento independiente — entrega local2026-11-18

Autorización: «implementa». Base `b6bb7e7c`; fuentes finales y sus huellas en el manifiesto del flujo del mismo cambio. **Código local verificado; sin instalar DLL/assets ni aplicar script09. Sin nueva prueba autenticada, consultas de negocio o efectos reales.**

Se implementan3.171–3.186,3.188–3.195 y3.197:25 tareas locales. Progreso204/217,13 pendientes: las once anteriores3.86/3.87/3.118/3.126/3.127/3.130/3.131/5.2/6.4/6.5/6.6, más las pruebas reales3.187/3.196. Complejidad27 S/190 M;81 escenarios normativos. Las pruebas locales no cierran carreras/rollback MySQL ni aceptación integral.

## Resultado entregado

- Almacenar A comprueba A, con sus permisos, expediente confirmado, revisión, índices, PDF y protocolo V2. Retira las dos pasadas cruzadas de originales. La raíz puede crearse con un original utilizable; conserva su creación única y no admite una raíz vacía.
- La procedencia de datos/valores del plan1/2 se valida una vez y se materializa como pruebas por archivo en eventos existentes. Referencias documentales confirmadas se proyectan en la misma tabla, bajo el lock de lote, para detectar duplicados sin reconstruir sus JSON en cada checkpoint. Un índice adicional manual; ninguna tabla/columna nueva.
- Reserva/checkpoints siguen siendo transaccionales y exclusivos. Incertidumbre sin retorno, marcador huérfano o recepción activa/incierta bloquean; un resultado duradero que acredita retorno puede aislarse para procesar otro ID, conservando su incertidumbre y recuperación.
- Original ausente, alterado o ilegible anterior a la invocación: negativo específico y diálogo propio con archivo/ruta/fase/causa/certeza/avance. Continuar excluye solo ese archivo durante la ejecución consentida; pausar, Escape o cerrar no despachan otro. Un resultado incierto no se transforma en ausencia de archivo.
- Lectura focal2 mantiene recurso/padre/autoridad/revisiones sin huella de todos los IDs por fase; lectura1 sigue disponible. Conteos intermedios por recursos/acuses conocidos, agregados SQL para estados y auditoría integral antes del cierre completo. Cancelaciones/bloqueos no se reactivan por consolidar otro archivo.

Contrato, compatibilidad y retorno: [contrato focal](../Diseno/contrato-focal-implementado-doc120.md). Flujo de guardado: [texto resumido](../Diseno/flujo-tecnico-importacion-carpetas-doc120.txt). Preparación operativa posterior: [ventana nueva](doc120-independientes-plan-real-20261118.md).

## Validación local y límites

258 pruebas Node únicas quedan aprobadas, incluidos2 wrappers Framework/DDL; no se suman sus casos internos ni las corridas superpuestas. La corrida secuencial de los28 archivos DOC120 ejecutó254 pruebas:253 pasaron y una detectó una expectativa histórica de deshabilitar Consultar último lote sin referencia. Esa expectativa ya contradecía la implementación aprobada en3.159, comprobada contra HEAD antes del cambio. Se corrigió para exigir el acceso habilitado; su archivo y la medición pasaron10/10 en la repetición focal. Dos pruebas adicionales de protocolo2, incluida lectura malformada que conserva el acuse y detiene el siguiente efecto, pasaron junto al conjunto de interfaz:17/17 en la repeticion final (9 casos anteriores superpuestos). Compatibilidad de retoma con lectura1:1/1 focal, sin sumar otra vez ese caso. El harness Framework/DDL final pasó2/2.

Compilación aislada `/t:Compile` del proyecto completo:0 errores, advertencias existentes; salidas en `.tmp/doc120-independent-build/`, sin sustituir `bin/` o IIS. OpenSpec estricto y auditoría de refinamiento15/15; `generate-doc120-flow --check`:124 fuentes/20.070 líneas en el anexo. Logs y capturas locales excluidos de Git; huellas en el JSON compañero.

| Escenarios | Evidencia local | Pendiente real |
|---|---|---|
| IND-01/02 | Harness Framework: guardado completo de A con B ausente, repetición sin segundo writer; creación y retención física local de A sin abrir B. Tres cargas de A en el recorrido de almacenamiento. | Composición MySQL/IIS final y corpus vigente. |
| IND-03/04 | Guardas de reserva, diario/token/ID/actor, negativo antes/después de reserva propia, ausencia de inicio/writer; estados/agregados focales. | Carrera, rollback y pérdida de commit en MySQL:3.187. |
| IND-05/06 | Matrices V2/época/acuse previas y nuevas pruebas de procedencia, identidad, no replay y consolidación; original propio retenido con bloqueo físico de escritura. | Fronteras reales de conexión/commit y retorno incierto. |
| IND-UI-01/02/03 | Core: A guardado/B ausente/C guardado tras decisión; pausa/cierre, sin decisión, File local ausente, incidencia ajena, sesión/detención/escritor activo. Chromium: foco, Tab, Enter sin decisión, Escape/cierre, doble elección y tamaños390/768/1280. | Interfaz autenticada con publicación/recursos vigentes. |
| IND-UI-04/05 | Retoma explícita no repite A/expediente, mantiene B pendiente y las selecciones; consulta/CSV/árbol/último lote y filtros en la regresión. Aviso explica que reseleccionar no reemplaza un RECIBIDO perdido. | No se certifica recuperación automática de un `.bin` perdido. |
| IND-PERF-01 | Inventarios1/3/100/1.000: una comprobación/retención focal por pasada, cero cargas/visitas de hermanos; pruebas de procedencia de hasta10 hashes hermanos para1.000. UI2 procesa1.000 con3.001 lecturas focales sin llamar a huella global. | Tiempos/SQL reales de la nueva DLL. |
| IND-PERF-02/COMP-01 | Parser saneado acepta nuevas categorías/bytes; ventana anterior sigue cerrada. Lecturas1/2, autoridad ajena, contrato inválido, fuentes y writer compartido/SII protegidos. | Instalación y comparación real3.196; once pendientes anteriores. |

Las mediciones locales separan fases: el ensayo Core de1/100/1.000 usa originales y valores ya registrados por dobles y realiza N almacenamientos sintéticos; su enumeración es lógica, sin HTTP/SQL. El ensayo Chromium2 sí recorre recepción/creación/materialización/almacenamiento por puertos sintéticos:1.000 documentos,3.001 lecturas focales, DOM máximo772, p95 de render10,7ms y aumento de heap25.234.095 bytes en la corrida registrada. No son tiempos de almacenamiento real ni una comparación porcentual con la versión anterior. Las cifras dependen del equipo y de la concurrencia de las pruebas.

El staging real conserva comprobaciones adicionales de A en V2. La métrica optativa distingue SQL ejecutado por los adaptadores de importación, JSON de control y accesos físicos instrumentados; no cuenta los SELECT internos del writer compartido. Agregados SQL pueden recorrer varias filas del lote. La inicialización del control y la auditoría final tienen costo propio, separado del camino normal por archivo.

## Aplicación posterior

[Script09 manual](../Sql/09-control-focal-indice-mysql51.sql) y metadatos esperados revisados; el harness rechaza índice incompleto, único, prefijado o fuera de orden. No se ha ejecutado el ALTER ni certificado su duración/bloqueos en el servidor MySQL5.1. Preparar el índice, instalar DLL/assets y realizar efectos necesitan la ventana específica descrita. Mantener respaldos/evidencias; no resetear filas, tokens o estados para forzar una retoma.
