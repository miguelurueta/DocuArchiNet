# DOC-120 - video final001055 y acceso al expediente

El video confirma en pantalla lote52 **COMPLETADO,1000/1000 guardados,0 pendientes y1 expediente**. Al pulsar Ver expediente aparece el aviso de nodo no visible o ambiguo y el formulario permanece abierto. Despues de cerrar y navegar manualmente por el arbol se cargan1000 archivos. Como varios expedientes muestran el mismo codigo, ese clic manual no identifica inequivocamente el ID696. No se muestra una nueva consulta del lote completado tras esa navegacion. [Procedencia, tiempos y fuentes](doc120-ver-expediente-video001055-20261120.json).

| Momento aproximado | Evidencia |
|---|---|
|00:00|Resultado completado1000/1000 y1 expediente.|
|00:01-00:29|Ver expediente deja el aviso; no abre el destino.|
|00:30-00:36|Cierra el formulario y navega por PRUEBA-IMPORT con codigos repetidos.|
|00:43|La cuadricula muestra Cargando.|
|00:50|Aparecen1000 archivos en CONTABIL. El video no expone el ID del nodo elegido.|

Se reproduce localmente con **OnSearchClick/AuTospam productivos y jQuery real**: Doc120MostrarExpediente localiza por relacion+nivel+ID, pero rechaza el enlace si sus ancestros nXNodes estan contraidos antes de llegar al buscador existente que los abre. Es una causa compatible con el video; no se capturo el DOM vivo ni SHA servido para atribuir el rechazo concreto de esa ventana a una condicion unica. La prueba anterior reemplazaba la busqueda por un registro de llamada y exigia rechazar incluso una rama contraida.

El acceso explicito ahora admite expandir solamente ramas WebForms nXNodes existentes dentro de un arbol visible, pasando TreeViewArchivo al buscador como en la seleccion manual. Mantiene rechazo ante arbol/contenedor ajeno oculto, identidad no representada o mas de un enlace logico para la misma relacion/nivel/expediente. No busca por codigo/nombre. No se cambia OnSearchClick, AuTospam, el postback existente de consulta de archivos, permisos, creacion o almacenamiento. Las consultas de seguimiento siguen conservando seleccion y expansion; no expanden por recibir un acuse.

Reproduccion previa1/2 (fallo de acceso esperado), focal10/10 y **101/101 finales en siete suites**, dos casos nuevos. Los diez focales se solapan con la final. Se acreditan apertura real de rama/seleccion del ID correcto con codigos iguales, consulta existente una vez y cierre de overlay; contenedores ocultos/duplicados rechazados antes de seleccionar, sin fases de importacion. Regresion de cliente, navegacion, barra/busqueda/carga individual pertinente, arbol, cierre y guardado sintetico de cuatro. No se suma QA nativa anterior ni se reejecuta la intermitencia contextual historica.

Cache del script compartido versionada doc120-acceso-expediente-20261120-2. Flujo textual/anexo/manifiesto actualizado:124 fuentes/20373 lineas e incluye Doc120MostrarExpediente, OnSearchClick, AuTospam y recorridos DOM auxiliares. Sintaxis/OpenSpec estricto/FLOW_CURRENT verificados.259/270 tareas,48 S/222 M,114 escenarios; once pendientes anteriores conservadas.

Cero HTTP/SELECT/sesiones/efectos reales durante esta revision/correccion, sin DLL/esquema/perfil o datos. La ventana7ace0d25 conserva PASS_COMPLETED_METADATA para1000 documentos distintos del expediente696/relacion1120 y estado local intacto. Falta comprobar visualmente Ver expediente tras recargar esta version y reabrir el lote completado; este ajuste local no cierra3.87 ni la aceptacion global. No repetir la importacion para comprobar el acceso.
