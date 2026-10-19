# Prompt 10 — Coordinación y recuperación del lote

## Encargo

Lee guía 00, prompt 03, diseño 04 y componentes reales 06–09. Implementa el servicio que coordina fases persistidas por unidad y archivo. No duplicar adaptadores, recepción ni repositorios.

## Trabajo

1. Definir operación acotada de avance: autorizar lote, cargar plan/configuración efectiva y reclamar una fase mediante estado/revisión persistidos. No procesar todo el lote en una petición larga ni iniciar tareas en memoria que prometan sobrevivir al reciclado.
2. Aplicar política publicada y ámbito confirmado. COMPLETO_OBLIGATORIO espera validación/recepción de todos los archivos de la unidad antes de crear/almacenar. PARCIAL_PERMITIDO procesa elegibles y mantiene rechazados/pendientes visibles. No reducir el inventario original para obtener COMPLETADO.
3. Ejecutar creación/verificación de expediente una vez por raíz y procesar documentos mediante 08/09. Revalidar permisos, destino y referencias antes del efecto; no confiar solo en el preview.
4. Persistir reclamaciones, intentos, transiciones y resultados. Una exclusión vencida no prueba que el efecto no ocurrió: reconciliar antes de repetir. Considerar dos clientes y una petición lenta que termina después de vencer su reclamación; no permitir efectos duplicados por un segundo ejecutor.
5. Clasificar reintentos: recepción segura, rechazo corregible, fase pendiente conocida e incertidumbre. No aplicar un reintento general a cualquier excepción. Acotar número/frecuencia y conservar diagnóstico seguro.
6. Implementar detención en puntos seguros. Cancelar pendientes no revierte efectos confirmados ni garantiza interrupción de llamadas en vuelo. Estados inciertos siguen requiriendo verificación.
7. Consolidar lote/raíz desde estados persistidos: completo solo con todos los efectos exigidos confirmados; parcial, detenido, fallido o pendiente de verificación con totales reconciliables. No derivar verdad de barras de progreso cliente.
8. Conservar snapshot y ámbito del nivel; cambios en perfil publicado o sesión no reescriben el lote. Revalidar disponibilidad externa conforme a contrato sin cambiar destinos silenciosamente.

## Primera modalidad de ejecución

Ambos ámbitos están aprobados: implementar una barrera parametrizada en el mismo coordinador. EXPEDIENTE evalúa todos los archivos de la raíz; LOTE evalúa todas las raíces/archivos del inventario confirmado. Dos solicitudes concurrentes no deben saltarse la barrera por un contador obsoleto: comprobar estados persistidos y habilitación bajo la coordinación transaccional definida. Mantener modo/ámbito del snapshot en reanudación; ninguna petición puede cambiarlos. Un fallo posterior conserva avances y bloquea completar la unidad; no hay rollback global ni garantía de éxito simultáneo.

Propiedad: avance, cancelación y reconciliación pertenecen a este servicio; 11 consulta/presenta sus acciones, no implementa otra máquina de estados. Si un permiso o el perfil se desactiva, preservar evidencias y resultados y aplicar la política explícita de suspensión de nuevos efectos sin sustituir versión. Autorizar consulta/reconciliación separadamente para permitir una investigación legítima; nunca habilitarla a un usuario que perdió autorización.

El cliente puede solicitar avances sucesivos y consultar estado. No prometer continuidad desatendida al cerrar el navegador. El servicio debe permitir que otro cliente autorizado recupere el mismo lote; los bytes locales no recibidos requieren selección nuevamente. Dejar el puerto de ejecución separado para 13 sin implementar una plataforma de workers ahora.

## Aceptación y entrega

Inyectar fallos antes/después de cada efecto, pérdida de respuesta, concurrencia, exclusión vencida, expiración de sesión, cancelación con llamada en vuelo y cambio de permisos. Verificar que completo espera la unidad exigida y nunca aparece completo con un archivo incierto. Probar reanudación sin recrear expediente ni reinsertar documentos confirmados. Entregar coordinador, transiciones implementadas, endpoints, pruebas y limitaciones de reconciliación. Cualquier cambio de esquema se entrega como SQL manual MySQL 5.1 con dependencias documentadas.
