# Diagnóstico de coherencia e integración de los prompts

Fecha: 2026-10-19. Revisión de arquitectura y contratos, con correcciones documentales. No es certificación de implementación ni autorización de despliegue.

## Dictamen

El alcance general es viable como extensión independiente de la importación SII, pero los documentos contenían contradicciones históricas y vacíos capaces de provocar implementaciones divergentes. Se corrigieron las inconsistencias documentales identificadas y se explicitó la propiedad de contratos, componentes y SQL. La integración funcional sigue condicionada a caracterizar las operaciones legacy y resolver las decisiones pendientes indicadas abajo.

No se puede afirmar «sin regresiones» solo por revisar prompts: la nueva capacidad aún no está implementada. SII intacto es una restricción obligatoria, no una conclusión de compatibilidad en ejecución. Invocar código sin modificarlo también puede afectar recursos compartidos, sesión, temporales o índices; esas interacciones necesitan evidencia.

## Alcance y evidencia

- Contraste de la guía 00, prompts 02–13, exploraciones 00–05 y evidencia de esquema referenciada; revisión del prototipo como contrato visual, no como servicio real.
- Relectura de puntos concretos en `Gestion/ClassGaExpediente.vb`, `Gestion/ClassGaProducionDocumental.vb`, `workflow/ClassAlmacenamiento.vb` y pruebas DOC-88. Las líneas citadas son las observadas en esta revisión y pueden desplazarse.
- No se ejecutó SQL, no se volvió a consultar la base ni se leyeron credenciales. No se hicieron cargas autenticadas, pruebas de volumen ni cambios de código de aplicación/SII/prototipo.
- Se ejecutaron seis archivos de pruebas estáticas DOC-88: **14 aprobadas, 0 fallidas**. Una de ellas comprueba la huella del handler compartido tras DOC-90. Esto no verifica transacciones, almacenamiento físico ni integración SII en ejecución.

## Matriz de diagnóstico

«Corregido documentalmente» significa que se eliminó o acotó una instrucción peligrosa/contradictoria; la prueba productiva requerida sigue indicada en cada fila.

| ID | Prioridad | Evidencia / documentos afectados | Inconsistencia o riesgo | Corrección realizada / condición restante |
|---|---|---|---|---|
| A01 | Alta | Exploración 00 §§5–6 frente a 01/03 y prompt 02 | Se mantenía como decisión abierta referenciar autorregistro, aunque ya se decidió perfil propio y existen efectos sobre edición de expediente. | **Corregido documentalmente:** perfil/versión propios con catálogos existentes; sin identidad autorregistro. Cualquier lectura histórica como plantilla no habilita escritura ni dependencia de SII. |
| A02 | Alta | Exploración 00 §7 y §12 frente a prompts 03/05 | El ejemplo de carpeta contenedora podía interpretarse como crear expedientes por hijas, distinto de una raíz seleccionada por expediente. | **Corregido documentalmente:** seleccionar cada raíz explícitamente; contenedor físico no equivale a estrategia de hijas. Subcarpetas no crean niveles de Producción. |
| A03 | Alta | Exploración 00 §§8/10/12 y 03 §6 frente a flujo aprobado | Origen, punto de entrada y ejecutor seguían abiertos en párrafos históricos; podían inducir selector adicional, servidor/red o worker obligatorio. | **Corregido documentalmente:** dispositivo → árbol existente/nivel → ventana de solo lectura; peticiones acotadas; servidor/red fuera del alcance inicial y 13 opcional. |
| A04 | Alta | Prompt 03, 05, 06, diagrama 04 y validador del prototipo | Bloquear por cualquier error individual haría imposible PARCIAL_PERMITIDO. La maqueta no refleja la política posterior. | **Corregido documentalmente:** separar errores globales/individuales y usar `puedeConfirmar` calculado por servicio. Prototipo identificado como referencia visual anterior a esa política. Probar parcial con elegibles y rechazados conservados. |
| A05 | Bloqueante del contrato afectado | Guía 00, prompts 02/04/06/10 | «Todos deben pasar» no fija si la barrera abarca expediente o lote. Asumir uno cambia creación y resultados. | **Resuelto por aprobación posterior del usuario:** soportar EXPEDIENTE y LOTE configurables en el perfil, con selección obligatoria al publicar en modo completo. No publicar una política ambigua ni afirmar que completo equivale a rollback global. |
| A06 | Alta | 06 necesita persistir plan; 07 declaraba depender de 06 | Dependencia circular de implementación que podía producir repositorios/DDL duplicados o planes solo en memoria por conveniencia. | **Corregido documentalmente:** 07-A crea fundaciones según contratos 04; luego 06; 07-B integra confirmación/recepción. Un solo propietario SQL/repositorios. |
| A07 | Media | Prompts 05/10/11 | Ventana, resultados y recuperación podían implementarse dos veces; faltaba dueño inequívoco de apertura y comandos. | **Corregido documentalmente:** 06 apertura/preflight; 05 UI base; 07 confirmación/recepción; 10 comandos; 11 consulta/exportación/integración de resultados sobre la misma ventana. |
| A08 | Crítica para recuperación | `workflow/ClassAlmacenamiento.vb:9126–9136`, prompts 07/09/11 | El método candidato elimina `MatrizDocumentosFinal` y temporal de sesión. «Limpiar solo con 11» no impediría esos borrados internos si se entrega el único staging. | **Regla corregida; prueba pendiente:** separar staging retenido de copia consumible por legacy, verificar fallo tras persistencia/borrado. Si no puede aislarse sin modificar código compartido, ese punto de reutilización queda no apto. |
| A09 | Alta | `workflow/ClassAlmacenamiento.vb:8938,9089,9122` | El nombre documental se deriva de `File.Name`; un temporal opaco puede reemplazar el nombre original aunque exista parámetro `NombreDocumento`. | **Regla corregida; prueba pendiente:** demostrar preservación del nombre/extensión con contrato soportado o copia segura en directorio único. Probar homónimos y Multi-TIFF. No cambiar el método compartido. |
| A10 | Alta | Plan previo 06 frente a creación 08/almacenamiento 09 | Índices heredados pueden depender de IDs/consecutivos de expedientes aún inexistentes. Exigir todos los valores finales en preview induce IDs inventados o creación anticipada. | **Corregido documentalmente:** fuentes diferidas tipadas, validadas en preflight y materializadas/revalidadas tras creación. Persistir procedencia/valor efectivo y fijar fechas generadas una vez. Diseño detallado pendiente en 04. |
| A11 | Crítica para reutilización | `ClassAlmacenamiento.vb:9026,9092–9093,9132–9136`; `ClassGaExpediente.vb`, creación y relación | Exclusión por lote no coordina escritores manuales/SII ni necesariamente el índice XML. `Finally` restaura variables, no efectos físicos. Una lease vencida no prueba que dejó de correr el efecto anterior. | **Barrera explícita añadida:** caracterizar aislamiento y correlación; ante incertidumbre no repetir hasta verificar. No prometer no duplicación usando solo CAS. Si exige cambiar SII/contratos compartidos, bloquear esa vía de adaptación. |
| A12 | Alta | Prompt 02 incluía protección HTTP; 03/07 no la explicitaban igual | Proteger solo administración del perfil deja confirmación, multipart y comandos sin contrato uniforme contra solicitudes falsificadas. | **Corregido documentalmente:** guía común exige autenticación, autorización por recurso y protección aplicable a todas las mutaciones. Pruebas de petición directa/manipulada en 12. |
| A13 | Alta | Snapshot inmutable frente a «perfil activo»/revalidación en 02/10 | Publicar otra versión o desactivar perfil podía cambiar una ejecución existente o impedir investigar un resultado incierto. | **Decisión aprobada 2026-10-20:** desactivar impide nuevas selecciones/confirmaciones; lotes confirmados continúan con snapshot y permisos vigentes. Solo detención explícita o pérdida de autorización impide nuevos efectos afectados. Contrato común 06; consulta/reconciliación autorizadas. |
| A14 | Alta | SQL manual frente a integración WebForms y proyecto VB | Desplegar interfaz/código antes de tablas podría romper toda la pantalla si carga esquema nuevo al iniciar; archivos nuevos pueden no incorporarse a compilación/publicación. | **Criterio añadido:** ausencia/incompatibilidad afecta solo nueva capacidad, no carga individual/Producción. Comprobar inclusión VB/recursos y orden manual de despliegue. No modificar límites globales ni activar migraciones automáticas. |
| A15 | Alta | Guía SQL y posibles DDL en 06/08–11 | Cada entrega podía producir estructura distinta o reescribir un script ya aplicado. | **Corregido documentalmente:** 02 posee SQL de perfil; 07-A operativo; restantes consumen y entregan incrementos si hay cambios ya desplegados. DDL MySQL 5.1 sin promesa de rollback; archivos todavía por implementar. |
| A16 | Media | Prototipo, exploración y pruebas previas | Aprobación visual y pruebas estáticas podían interpretarse como cobertura de recepción completa, concurrencia o integración documental. | **Límite explicitado:** HTML simula; DOC-88 estático. El prompt 12 exige integración real autorizada y no permite cambiar huellas/expectativas para ocultar regresiones. |
| A17 | Bloqueante de capacidad/operación | Exploración 03 §6; prompts 04/11/12 | Volumen, límites reales, retención, navegadores, código único, fechas, responsables/edición y permisos concretos no se deducen del prototipo ni del esquema. | **Pendiente:** decisiones en 04 y validación de entorno autorizado en 12. No fijar 25 MB/500 archivos ni plazos de borrado de ejemplo como producto. Se puede avanzar en componentes independientes. |

## Matriz de integración después de las correcciones

| Entrega | Produce | Recibe / utiliza | No debe hacer |
|---|---|---|---|
| 04 | Diseño común y decisiones | Evidencia y requisitos vigentes | Implementar componentes o ejecutar DDL. |
| 02 | Perfil/versiones/reglas/resolvedor; SQL 01 | Diseño 04, catálogos | Crear lotes ni fijar otro nivel de destino. |
| 07-A | SQL 02, repositorios de plan/operación | Contratos 04 y perfil | Esperar a 06 para definir tablas que 06 necesita. |
| 06 | Contexto autorizado y plan | Perfil 02, repositorios 07-A | Crear expedientes para completar IDs de preview. |
| 07-B | Lote confirmado y recepción | Plan/revalidación 06 y 07-A | Almacenar documentos finales o ceder el único staging consumible. |
| 08/09 | Efectos y verificaciones documentales | Comandos, repositorios y archivos habilitados | Saltar coordinador, cambiar SII o duplicar escritores/relaciones. |
| 10 | Avance/cancelación/reconciliación | Plan, repositorios y adaptadores | Crear otro motor de reglas o asumir worker desatendido. |
| 05/11 | Ventana conectada, consulta, CSV y árbol | Contratos/acciones reales del servidor | Decidir permisos, marcar éxito ficticio o duplicar estados del servidor. |
| 12 | Evidencia de integración/regresión | Implementación y entorno autorizado | Convertir pruebas no ejecutadas en aprobadas. |
| 13 | Ejecutor durable opcional | Coordinador y adaptadores sin sesión | Ejecutarse implícitamente o inventar HttpContext para afirmar independencia. |

## Condiciones para considerar lista la implementación

1. Aplicar A05 resuelto (ambos ámbitos configurables) y resolver las demás decisiones de negocio/operación que afecten el contrato de cada entrega; registrar lo restante como pendiente, sin valores silenciosos.
2. Caracterizar A08–A11 antes de declarar reutilizables los adaptadores. Los métodos candidatos no se consideran aprobados por esta revisión.
3. Entregar SQL coherente con contratos y repositorios, revisar su aplicación manual y degradación controlada si falta esquema.
4. Probar modo completo/parcial, contexto fijo del nivel, valores diferidos, nombre original, sesión, temporales, concurrencia y ventanas de fallo.
5. Demostrar regresiones pertinentes en entorno autorizado y preservar la restricción de SII. No editar pruebas de huella para aceptar cambios prohibidos.

## Verificación ejecutada en esta revisión

```text
node --test tests/doc88-produccion-carga-almacenamiento.test.cjs
            tests/doc88-produccion-carga-configuracion.test.cjs
            tests/doc88-produccion-carga-preparacion.test.cjs
            tests/doc88-produccion-carga-tipologias.test.cjs
            tests/doc88-produccion-proyeccion.test.cjs
            tests/doc88-produccion-upload-contract.test.cjs

14 aprobadas · 0 fallidas · 0 omitidas
```

El comando se ejecutó como una sola invocación con los seis archivos. Su resultado es evidencia del código actual; no demuestra que una implementación futura de los prompts no introduzca regresiones. Se comprobaron además los enlaces locales de 21 documentos Markdown: 0 rotos; 0 líneas con espacios finales. Las modificaciones de esta revisión son exclusivamente documentales.
