# Prompt PB00 para inventariar los contratos del explorador

**Estado:** preparado para revisión y ejecución de inventario documental. No implementa aplicación ni aprueba decisiones.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior de ASP.NET WebForms, VB.NET sobre .NET Framework 4.6.1 y JavaScript/jQuery. Respeta los contratos vigentes de Producción documental y el alcance específico del paquete.

## Refinamiento e implementación del paquete

Comienza por refinar las decisiones indicadas en este archivo y sus dependencias, sin exigir que estén cerradas las 18 decisiones de la exploración. Relee el código y los resultados anteriores, delimita el alcance y registra en la exploración la opción, evidencia y revisión que realmente se acuerden. Los detalles técnicos dentro del alcance autorizado se justifican; los cambios de comportamiento, permisos, semántica o alcance requieren aprobación del usuario.

Una decisión pendiente no impide iniciar este refinamiento. Antes de modificar el código afectado debe quedar resuelta la decisión necesaria y existir autorización de implementación. Continúa el análisis independiente si una respuesta sigue pendiente. No conviertas las alternativas recomendadas en aprobaciones ni vuelvas a pedir acuerdos ya registrados. Después implementa y verifica solo la parte habilitada.

## RESTRICCIONES CRÍTICAS

- Preservar identidad, nombres canónicos, reglas de negocio, permisos y formatos públicos de los consumidores vigentes. No romper la búsqueda, la selección ni los accesos alternativos para cumplir solo el diseño visual.
- No sustituir el TreeView, migrar framework, crear tablas/endpoints o ampliar permisos como efecto implícito del paquete. Toda necesidad fuera de alcance se refina y registra antes de implementarla.
- No ejecutar pruebas autenticadas, carga ni escrituras del ambiente sin su autorización específica. No imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión en prompts o evidencia versionada. Los SELECT de control no modifican dominio ni auditoría documental.

## OBJETIVO

Actúa como arquitecto de ASP.NET WebForms y JavaScript. Lee [el contexto común](contexto-comun.md) y caracteriza los contratos del árbol actual para que cada cambio posterior tenga consumidores, comportamiento y verificaciones concretos. El resultado debe permitir implementar sin deducir reglas a partir de los colores o de los datos simulados de la maqueta.

El alcance es solo documentación sin cambios de código. No crea modelos, DTOs ni servicios de aplicación.

## Insumos y archivos a revisar

Lee la exploración y revisa, por nombre de función, estos archivos desde la raíz:

- `Gestion/WebFormProducionDocumental.aspx` y su `.aspx.vb`.
- `Gestion/Class_niveles_organizacion.vb` y `Gestion/Class_ra_pro_niveles_has_expediente_archivo.vb`.
- `Gestion/ClassGaProducionDocumental.vb` y `Gestion/ClassGaExpediente.vb` para las operaciones invocadas.
- `webservice/WebServiceProducion.asmx.vb` y los servicios de permisos/archivos realmente alcanzados.
- `js/gestion/WebFormProducionDocumental.js` y `js/jquery.contextMenu.js`.
- Integración `Doc119NivelPulsado`, `Doc111ActualizarRama`, `Doc120MostrarExpediente` y separación de formularios de Excel.

## Trabajo requerido

1. Documenta el recorrido exacto desde clic, teclado, búsqueda y menú hasta campos ocultos, postback/ASMX, sesión, validación y respuesta.
2. Inventaría cada operación del diagnóstico. Por fila registra entrada visual, handler/servicio, tipo aplicable, identidad requerida, permisos efectivos, variables de sesión leídas/escritas, efecto de dominio y formato de respuesta. Distingue hechos por código de comprobaciones de ambiente pendientes.
3. Caracteriza al menos el nivel vacío/propio/recibido/compartido y el expediente con identidad compuesta. Incluye nodos iniciales y dinámicos y enlaces de imagen/texto para un mismo nodo.
4. Verifica dónde se toma la primera relación del expediente, dónde se cambia la relación al mover y qué elimina exactamente la operación de dominio. No propongas una regla de unicidad sin evidencia.
5. Revisa cómo se confirma la selección y qué ocurre al limpiar, fallar el postback, usar clic derecho sobre otro nodo o compartir sesión entre pestañas. Identifica qué rutas requieren cambios en PB01/PB02 y cuáles solo necesitan presentación.
6. Registra una lista finita de consumidores a preservar: búsqueda, expansión, creación/edición, traslado, ocultos, importación, archivo físico, rótulos, estados, índices, carga, archivos y acciones múltiples. Señala cualquier ruta que no se haya podido trazar.
7. Propón los contratos mínimos para los paquetes siguientes sin implementarlos: datos de identidad, autoridad, correlación y errores. Relaciónalos con decisiones pendientes, sin elegir firmas definitivas ni endpoints por anticipado.

## Verificación

Comprueba las suites locales existentes antes de invocarlas y ejecuta las que sean independientes del ambiente para fijar la línea base. Como candidatas:

```text
node --test tests/doc119-context-menu.test.cjs tests/doc120-importacion-arbol.test.cjs
node --test tests/doc126-importacion-excel-ui.test.cjs
```

No ejecutes pruebas autenticadas ni carga. Un fallo de la línea base se registra con su causa conocida y efecto sobre la implementación; no se corrige aplicación dentro de este inventario.

## CRITERIOS DE ACEPTACIÓN

- Cada familia del inventario tiene entradas, validaciones, resultados y consumidores localizados.
- Se conoce qué necesita backend y qué se resuelve en cliente.
- Las dudas de ambiente tienen dato requerido y comprobación propuesta, no una respuesta inventada.
- El contrato propuesto identifica las decisiones que lo habilitan y cualquier incompatibilidad con las alternativas.
- No se modifica código, base de datos, decisiones ni el cambio OpenSpec DOC-127 como consecuencia del inventario.

## ENTREGABLE FINAL

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB00-contratos-vigentes.md` con el mapa de operaciones, evidencia por función, línea base, riesgos y dependencias. Actualiza los vínculos documentales pertinentes y registra únicamente estos cambios de documentación. El paquete termina con una lista concreta de entradas para PF01 y PB01, no con una declaración de implementación realizada.

## PRUEBAS OBLIGATORIAS

Este paquete es solo documentación sin cambios de código de aplicación. E2E no aplica a esta entrega documental porque no expone un recorrido de usuario ni un endpoint nuevo. La caracterización de recorridos existentes y sus necesidades de ambiente permanece en el inventario y no se presenta como validación real.

No aplica build MSBuild del backend mientras no cambien aplicación o proyecto. Ejecuta los test locales de infraestructura que el paquete identifica, comprueba vínculos y contratos y realiza revisión manual de sus tablas. Registra comando, resultado y limitaciones; no ejecutes E2E autenticado ni una mutación real para completar el inventario.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad: exploración, guía, contratos del paquete y documento de resultado indicado en ENTREGABLE FINAL. Documenta funciones creadas o modificadas con ruta, parámetros, retorno, errores y consumidores cuando corresponda. Incluye decisiones aplicadas, evidencia saneada, compatibilidad y pendientes; no dupliques las aprobaciones fuera de la exploración ni des por existentes los resultados de paquetes futuros.
