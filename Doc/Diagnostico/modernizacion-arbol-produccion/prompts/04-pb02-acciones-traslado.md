# Prompt PB02 para ejecutar acciones sobre un destino comprobado

**Estado:** borrador de implementación. Requiere PB00 y PB01 verificados; decisiones de PB01 y D07, D11, D12 aprobadas para las operaciones incluidas.

## Objetivo

Actúa como desarrollador senior de ASP.NET WebForms, VB.NET y persistencia transaccional. Aplica [el contexto común](contexto-comun.md) y conecta las acciones aprobadas al contrato de identidad/autoridad de PB01. Mantén las reglas existentes y elimina, en las rutas incluidas, la posibilidad de ejecutar silenciosamente sobre una selección de sesión diferente del destino solicitado.

## Áreas y alcance finito

Parte del mapa de consumidores de PB00. Revisa `Gestion/WebFormProducionDocumental.aspx.vb`, `webservice/WebServiceProducion.asmx.vb`, `Gestion/Class_niveles_organizacion.vb`, `Gestion/ClassGaExpediente.vb` y las funciones de cliente que disparan esas operaciones.

Incluye crear/editar nivel y expediente, cortar/pegar, ocultar/restaurar y eliminar cuando estén dentro de la entrega aprobada. Identifica también accesos duplicados desde barra, menú contextual y botones ocultos. Los accesos de archivo físico, rótulos, índice, estados y documentos deben figurar en el mapa: adapta los que comparten el contrato ahora y deja una dependencia explícita de PB04 para los restantes. No declares completa una familia dejando activo un camino que use otro destino.

## Implementación de acciones

1. Para cada operación, valida tipo e identidad y su autoridad vigente mediante el contrato PB01; conserva además las comprobaciones de negocio específicas del handler existente.
2. Invoca el flujo de dominio sobre esa misma identidad dentro de la petición. Si el legacy requiere sesión, limita y documenta su uso en el adaptador; no bases la seguridad en un postback previo de preselección. Prueba solicitudes de dos páginas con destinos distintos.
3. Conserva formas de respuesta de accesos legacy. Un resultado nuevo debe distinguir rechazo, éxito confirmado e incertidumbre; separa un fallo de renderizado posterior de un fracaso de dominio.
4. Mantén la creación de expediente desde un nivel y desde un expediente, con destino de nivel explícito y sin tomar la primera relación por accidente. Crear subnivel sobre expediente continúa sujeto al rechazo actual.
5. Actualiza búsqueda, identidad de nodo, selección y contadores solo a partir de éxitos confirmados. Evita arreglos de DOM que aparenten éxito cuando falló el servidor.

## Traslado

Conserva `Get_cortar_expediente`, `Get_pegar_expediente` y `Traslado_carpeta_nivel` como contratos a adaptar o envolver con compatibilidad según PB00. No conviertas un nivel en elemento trasladable.

- Respeta propiedad de origen y destino, mismo nivel y relación ya existente; no sustituyas propiedad por una capacidad genérica de escritura.
- Comprueba origen, relación fuente y expediente vigentes al ejecutar. La referencia capturada al cortar puede haber quedado obsoleta.
- Mantén la unidad transaccional de alta de relación destino, eliminación de relación fuente y ajustes de contadores que corresponden a las reglas actuales. Verifica cardinalidades y manejo de error/rollback; una relación inexistente no se anuncia como trasladada.
- Devuelve identidad confirmada del expediente y su nueva relación. La UI no debe seguir usando la relación vieja ni regenerar un expediente.
- Ante pérdida de respuesta, ofrece comprobación del estado origen/destino con el alcance autorizado antes de repetir. No crees un registro durable de idempotencia sin una necesidad de persistencia aprobada ni prometas exactamente una ejecución sin demostrarla.

## Ocultar y eliminar

Ocultar nivel conserva su flujo de visibilidad/restauración. Eliminar utiliza la operación de dominio y todas sus restricciones, incluyendo hijos, relaciones o volúmenes cuando corresponda. El nombre del comando y la confirmación deben reflejar ese efecto; «Quitar del árbol» no sustituye una eliminación real.

Una cancelación no llama al servicio. Un rechazo conserva el estado local anterior. Un resultado incierto mantiene la advertencia y habilita verificación compatible; no elimina filas por suposición. Revalida permisos al ejecutar aunque el menú se hubiera abierto antes de una revocación.

## Verificación de aceptación

- Acción sobre B cuando A estaba seleccionado, desde cada acceso vigente y en dos páginas/pestañas.
- Crear sobre destino correcto; relación ambigua/obsoleta; expediente con nombre repetido.
- Cortar sobre origen no propio, pegar en destino no propio, mismo nivel, relación duplicada y fuente retirada después de cortar.
- Traslado exitoso cambia la relación y conserva el expediente; fallo intermedio no deja un éxito falso ni contadores anunciados incorrectamente.
- Cancelación/rechazo de eliminación y sus restricciones de dominio; ocultar/restaurar separado.
- Respuesta perdida y fallo visual después del éxito no ocasionan reenvío automático.

Utiliza pruebas de comportamiento y de servicio/transacción con recursos de prueba aislados o dependencias sustituibles. No ejecutes mutaciones en la base operativa para verificar el prompt. Compila el backend y registra cualquier verificación que requiera ambiente autorizado.

## Entrega

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB02-acciones.md` con matriz por acción/acceso, reglas preservadas, firmas y respuestas, transacción, pruebas y rutas que consumirá PB04. Entrega el código integrado y la evidencia; deja contratos suficientes para que PF02 renderice resultados confirmados sin otra interpretación de identidad.
