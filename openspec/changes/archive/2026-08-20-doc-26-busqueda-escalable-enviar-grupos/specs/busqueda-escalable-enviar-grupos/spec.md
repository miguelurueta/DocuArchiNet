<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## ADDED Requirements

### Requirement: D-01 Buscar destinos en un método de solo lectura

El sistema SHALL exponer BuscarDestinosEnvioGrupo en el ASMX moderno para consultar destinos paginados de Enviar a grupo. SHALL conservar las firmas de PreviewEnviarGrupo y EjecutarEnvioGrupo. PreviewEnviarGrupo SHALL devolver como máximo la primera página aplicada para evitar una descarga ilimitada al abrir el modal.

#### Scenario: Buscar con contrato aislado

- **WHEN** el cliente moderno solicita destinos con idTarea, termino, pagina y tamanoPagina
- **THEN** el servidor procesa la búsqueda en la operación nueva, el preview devuelve una primera página acotada y la ejecución conserva su contrato existente.

### Requirement: D-02 Devolver un contrato mínimo y sanitizado

El sistema SHALL devolver IdTarea, TokenVersion, Pagina, TamanoPagina, TieneMas, destinos permitidos y error público normalizado. El preview SHALL publicar Pagina, TamanoPagina y TieneMas de su carga inicial. Un destino SHALL incluir IdActividadDestino, NombreActividad y GrupoDestino resumido, sin IdConector ni datos de sesión, SQL, permisos o excepciones.

#### Scenario: Respuesta autorizada

- **WHEN** el contexto y la solicitud son válidos
- **THEN** la respuesta contiene solo datos de presentación de la ruta autorizada y los metadatos de paginación aplicados.

### Requirement: D-03 Aplicar límites de entrada explícitos

El sistema SHALL aceptar término vacío o de 2 a 80 caracteres, SHALL normalizar pagina a un mínimo de 1 y tamanoPagina al intervalo 1..50, y SHALL usar 25 como tamaño por defecto. Un término no vacío fuera del rango SHALL recibir un bloqueo funcional seguro.

#### Scenario: Parámetros fuera de rango

- **WHEN** pagina es cero, negativa o tamanoPagina excede el máximo
- **THEN** la respuesta identifica los valores normalizados y nunca produce una consulta sin límite.

### Requirement: D-04 Reautorizar cada lectura y limitarla a SELECT

El sistema SHALL evaluar sesión, Cambio_Ruta, tarea activa, ruta y flujo aplicable antes de revelar resultados. La política moderna oficial no SHALL crear una restricción adicional por gate de despliegue. El repositorio SHALL ejecutar exclusivamente SELECT parametrizados para ruta, término, límite y desplazamiento.

#### Scenario: Usuario o tarea no autorizados

- **WHEN** el contexto no es válido, Cambio_Ruta no está concedido, la tarea no está activa o su ruta o flujo no están disponibles
- **THEN** el método devuelve un bloqueo público sin destinos y sin modificar tarea, estado, auditoría, eventos o configuración.

### Requirement: D-05 Buscar por actividad o grupo sin duplicar actividades

El sistema SHALL encontrar una actividad por NombreActividad o por un grupo relacionado en grupos_workflow, y SHALL retornar como máximo una entrada por IdActividadDestino. Para varias asociaciones SHALL publicar un resumen acotado en GrupoDestino; el grupo no será un identificador de selección.

#### Scenario: Actividad asociada a varios grupos

- **WHEN** el término coincide con cualquiera de los grupos de una misma actividad
- **THEN** la respuesta contiene una sola actividad seleccionable y su resumen de grupos.

### Requirement: D-06 Paginar con una fila adicional

El sistema SHALL ordenar resultados de forma estable y solicitar tamanoPagina más una fila para determinar TieneMas. SHALL evitar COUNT por pulsación y SHALL requerir una decisión y migración aprobadas antes de crear índices o modificar el esquema.

#### Scenario: Existe página siguiente

- **WHEN** hay más resultados que el tamaño aplicado
- **THEN** la respuesta contiene como máximo TamanoPagina destinos y TieneMas es verdadero.

### Requirement: D-07 Mantener resultados y selección consistentes en la UI

El cliente SHALL iniciar la búsqueda después de 300 ms de inactividad para términos de dos o más caracteres, restaurar página uno al limpiar y anunciar cargando, resultados, vacío o error recuperable mediante aria-live. SHALL cancelar o descartar respuestas obsoletas e invalidar selección ante cambio de filtro, página, reintento o preview.

#### Scenario: Respuesta tardía

- **WHEN** una búsqueda anterior termina después de una búsqueda más reciente
- **THEN** tabla, tarjetas, paginador y confirmación conservan únicamente el conjunto más reciente.

### Requirement: D-08 Conservar ejecución y accesibilidad modernas

El sistema SHALL conservar el payload exacto de ejecución { idTarea, idActividadDestino, tokenVersion }, la relectura y revalidación dentro del lock, y el adaptador moderno actual. SHALL conservar foco, Escape, trampa de foco, teclado y prevención de doble clic. Para un contexto Workflow válido no SHALL entregar postback legacy de Enviar a grupo; Continuar flujo SHALL conservar IdConector y sus endpoints modernos.

#### Scenario: Ejecución después de una búsqueda

- **WHEN** el token vence, el destino se retira o existe concurrencia al confirmar
- **THEN** EjecutarEnvioGrupo bloquea de forma segura y la búsqueda no sustituye ninguna de sus revalidaciones.

### Requirement: D-09 Restablecer la bandeja después de un envío confirmado

El cliente SHALL, después de aplicar el éxito correlacionado de un envío moderno, ocultar el contexto de tarea, mostrar el listado, restablecer el scroll horizontal del contenedor de la bandeja al inicio y recalcular su layout con la rutina existente. No SHALL generar una petición adicional, un postback ni modificar el payload, el gate o Continuar flujo.

#### Scenario: Éxito de envío a grupo desde una bandeja desplazada

- **WHEN** EjecutarEnvioGrupo confirma la tarea seleccionada y la bandeja tenía desplazamiento horizontal
- **THEN** se retira únicamente esa fila, el contexto anterior queda oculto y la bandeja visible conserva paginador, controles y scroll horizontal desde la primera columna.
