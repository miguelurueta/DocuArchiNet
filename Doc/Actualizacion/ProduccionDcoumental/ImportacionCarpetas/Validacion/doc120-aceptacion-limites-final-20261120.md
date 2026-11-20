# DOC-120 — matriz final de aceptación y límites (6.4)

Revisión técnica solicitada al continuar6.4–6.6, después de3.187. Base revisada `aee3fe8eb5023546b134266c6e14ff66b94f8a02`; no se repiten cargas ni se modifican datos/runtime. **Dictamen técnico favorable con los límites siguientes; aceptación operativa expresa pendiente.** La instrucción de continuar autoriza esta revisión; no se transforma en una declaración del usuario sobre límites que todavía no había revisado en conjunto.

## Resultados verificados y procedencia

| Área | Evidencia efectiva | Decisión técnica |
| --- | --- | --- |
| Flujo, privacidad, acuse perdido y continuidad | E2E5f7af8f5 y08bc33cc; confirmación manual del usuario de3.87/3.127; regresión local final226/226. | No se detecta fallo crítico abierto de autoridad/destino/integridad/recuperación en el alcance comprobado. No se reejecutan efectos ya confirmados. |
| Importación completa | Control real7ace0d25: lote52,1.000 documentos/inventarios únicos,44.859.102 bytes, expediente696/relación1120, sin intentos sin resolver. Inspección83b36216 confirma además1.000 completados en50. | Aceptar la importación funcional observada; no volver a cargar el corpus. |
| Rendimiento | Cuatro CSV reales conservados por versión: medias79,432043/1,058143/0,969465/0,980750 s; último backend igual al instalado. | Rendimiento muestral ya determinado. No multiplicar una media por1.000 ni inventar tiempo integral/memoria/disco. |
| Persistencia segmentada |1a31695b, preparación9/plan93/lote56, carreras/rollbacks MySQL y conservación comprobada. | Mantener PASS en su alcance nativo y cobertura local complementaria, sin convertirlo en dos pestañas reales. |
| Reservas por archivo |655467f4/código6fc5d343, lote57: dos conexiones/un ganador, revisión antigua, B reservado, tres rollbacks/cierres;17 grupos previos/5 del fixture/154 fuentes conservados. |3.187 completa; ningún adaptador de almacenamiento invocado por los casos nativos. |
| BENEFICIARIO | Confirmación del usuario de tabla/perfil ampliados; video111928 muestra máximo255 y operación completada. | Conservar aceptación manual y255 observado. No inventar ALTER ejecutado por el agente, consulta SQL posterior o ID de publicación. |

## Límites y propuesta explícita de salida

| Límite | Motivo y alcance que falta | Responsable | Propuesta de decisión de salida |
| --- | --- | --- | --- |
| CAPACIDAD/SLA | No se midieron duración integral de todas las fases, máximo de memoria/disco, archivos grandes, saturación o capacidad máxima.1.000 funcionales y latencias muestrales sí están acreditados. | Solicitante para aceptación; operador para presupuesto de una carga futura. | Entregar el flujo comprobado sin SLA ni garantía de capacidad máxima; conservar cuotas y guardas existentes. Una prueba distinta se presupuesta/autoriza aparte, sin imponerla para repetir lo ya medido. |
| SII-REAL | No hay ejecución SII real nueva; las cargas verificadas corresponden a Producción Documental. La ruta compartida mantiene su compatibilidad comprobada local/históricamente. | Solicitante/operador de SII. | Entregar DOC-120 sin certificar SII real ni habilitarlo por inferencia. Ninguna invocación SII para cerrar este ticket. |
| LIMPIEZA-REAL | No se ejecutó borrado real ni se midió espacio disponible actual. El inventario histórico57 archivos/24.371 bytes no describe el disco de hoy. | Solicitante/operador del almacenamiento. | Conservar originales y evidencias; no activar limpieza, borrar fixtures o prometer espacio liberado. Cualquier limpieza futura requiere recursos y autorización propios. |
| UI/RUNTIME | Cobertura mixta: E2E reales y confirmación manual en sus cortes; ajustes finales de encabezado/árbol cubiertos localmente. La prueba08bc33cc conserva sus huellas; Vista/ASPX cambiaron después, por lo que no se reetiqueta esa sesión como ejecución del cliente final. | Solicitante para aceptar cobertura; Codex para trazabilidad/regresión local. | Aceptar cobertura mixta y226/226 locales sin exigir otra importación. No afirmar una nueva sesión IIS por cada commit documental ni una inspección del ensamblado cargado en memoria. |
| CONCURRENCIA |3.187 usa conexiones nativas; B activo significa RESERVADO. Hay E2E HTTP de dos sesiones en5f7af8f5, con su alcance distinto. | Codex/revisor técnico; operador para una nueva variante. | Certificar exclusión/rollback nativos y el HTTP efectivamente observado; no anunciar dos pestañas o un adaptador artificialmente mantenido en vuelo. |
| HISTORIA/INCERTIDUMBRE | Fixtures inciertos de laboratorio anteriores conservan sus propios estados y límites; no son autorización para repetición o limpieza. | Operador y responsable de aceptación. | Conservarlos sin replay ni liberación por timeout; no transformar antecedentes STOPPED/INCIERTO en PASS actuales. |

Las dos últimas filas fijan la lectura del alcance ya verificado, sin reabrir3.118/3.187. Los resultados funcionales/manuales anteriores no se solicitan de nuevo. La aceptación solicitada se limita a esta decisión conjunta de salida;6.4 sigue abierta hasta esa respuesta, y archivo/cierre esperan además el merge verificado.

## Fundamento de la compuerta

El diseño aprobado, D-10/RQ-10, exige: **«Límites necesitan aceptación expresa»** y que autoridad/destino/integridad/recuperación sin resolver bloqueen salida. No se usa CI, una casilla o una confirmación del plan local como sustituto. [Diseño](../../../../../openspec/changes/doc-120-produccion-importacion-carga-carpeta-verificacion/design.md).
