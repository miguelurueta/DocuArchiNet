# DOC-111: autoridad y conexión previas a creación legacy, 2026-10-29

Avance parcial de 9.3, D-02/D-07, RQ-02/RQ-07. **95/130 tareas completas, sin incremento**. No se invoca creación ni almacenamiento y no se completa el adaptador.

Se amplía el ContextAdapter existente con AutorizarCreacionLegacy. Comprueba que la petición sea HttpContext.Current, valida las entradas del comando y compara las opciones efectivas de dos builders MySQL: el resuelto por ModuleSessionConnectionStringResolver y el construido en memoria con los mismos argumentos de la función legacy RA, en conect:38–52. No abre ninguna conexión para esta comparación, no devuelve cadenas y no guarda/imprime credenciales. Los errores de construcción se sustituyen por el contrato saneado de indisponibilidad.

La única diferencia admitida expresamente es PersistSecurityInfo=True, que legacy fija. Las diferencias efectivas de destino, identidad de conexión o pooling se rechazan. Un máximo de pool vacío que ambos builders interpretan con el mismo defecto sigue siendo equivalente: no se modifica la configuración ni se inventa un límite funcional. No se alteran el resolvedor o conect.

Después reutiliza AutorizarRecursos/CREAR_EXPEDIENTE, que vuelve a comprobar usuario activo, empresa y facultad vigentes, permiso del nivel, cadena/sello y origen RA, identidad DA recíproca y gabinete. Un valor de permiso anterior en Session o el comando no reemplaza estas lecturas. No se cambia Session ni se utiliza una selección global como destino.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: familias anteriores y casos Framework con HttpContext/Session sintéticos y SELECT respondidos por el doble existente. Acepta contexto y opciones equivalentes sin mutar Session; rechaza revocación, cambio de usuario/empresa, traslado de nivel/origen, gabinete o identidad DA denegados y otro HttpContext.Current.
- Casos de cadenas sintéticas incompatibles en base/servidor/usuario/contraseña, pooling divergente, proveedor no compatible y máximo de pool inválido: rechazo antes de cualquier consulta RA/DA. Se comprueba también el defecto equivalente de pool. Los textos de prueba son ficticios; no se usa configuración real ni se imprime su contenido.
- Build Debug completo sin despliegue, salida/intermedios `.tmp/doc111-contexto-legacy-build/{bin,obj}`, log `.tmp/doc111-contexto-legacy-build.log`, código 0.
- OpenSpec estricto y revisión de diferencias PASS; ClassGaExpediente, ClassAlmacenamiento, ClassNeodynamic y conect sin cambios.

Límites: la equivalencia de builders no prueba conectividad o efectos MySQL reales. La autoridad conserva su comprobación de origen, pero las lecturas no proporcionan exclusión frente a cambios posteriores ni sustituyen la reclamación/barrera del coordinador. Aún falta el ciclo de archivo, composición final de argumentos, invocación compatible y comprobación/correlación del resultado. Las dependencias transitivas de legacy no se dan por probadas por agregar esta preparación.

Sin SQL/HTTP/E2E reales, efectos documentales, limpieza, cambios de datos/configuración o despliegue. SII y almacenamiento permanecen intactos.
