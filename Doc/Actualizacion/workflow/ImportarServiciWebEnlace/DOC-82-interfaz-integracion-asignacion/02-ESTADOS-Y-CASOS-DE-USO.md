# Estados y casos de uso implementados

## Máquina visible

`Cerrado → Consultando → Vacío | Resultados | Error → Vista previa | Preparando → Ejecutando → Reconciliando → Completado | Parcial | Incierto | Fallido`.

- Durante ejecución/reconciliación, X, Escape y backdrop no cierran.
- Éxito total confirmado: sincroniza, espera el refresco y cierra.
- Parcial, incierto o fallido: mantiene el modal abierto.
- Cambiar de tarea invalida el contexto capturado; una respuesta tardía no se proyecta.
- La asignación no forma parte de esta máquina: ocurre después, por acción explícita, y se revalida en servidor.

## Casos de uso

| ID | Actor | Precondiciones | Flujo principal | Alternativas/errores | Resultado |
|---|---|---|---|---|---|
| CU-01 | Usuario Workflow | Sesión, gate activo, tarea ENLASE seleccionada | Abre `a_adj_service_web`; resuelve capacidad y consulta | Capacidad/proveedor no disponible → error seguro | Tabla 0/1/N |
| CU-02 | Usuario Workflow | Resultado con acción Preview | Solicita `GetPreview`; muestra descriptor mediado; vuelve a fila | Vencido/no visualizable/no autorizado → estado específico | Selección, scroll y foco conservados |
| CU-03 | Usuario Workflow | Uno o más anexos importables | Selecciona uno/todos; prepara; completa tipología; confirma | Catálogo vacío/ambiguo o preflight inválido → no crea intención | Una intención con N Items |
| CU-04 | Usuario Workflow | Intención creada y contexto vigente | Ejecuta una vez, consulta y reconcilia | Parcial/incierto/error → permanece abierto; cambio de tarea → bloquea proyección | Documentos confirmados, sin duplicados |
| CU-05 | Usuario Workflow | Éxito total reconciliado | Refresca lista documental y cierra | Refresco pendiente → espera; no confirmado → no cierra | Página anfitriona íntegra |
| CU-06 | Usuario Workflow | Importador cerrado | Pulsa Asignar | Documento obligatorio ausente o error distinto de `YES` → mensaje y `Exit Sub` | Backend decide si continúa la asignación |

## Accesibilidad y responsive

El diálogo conserva foco inicial y retorno al disparador; preview/preparación devuelven foco a la acción origen; Tab permanece en el diálogo; Escape respeta el lock. La tabla usa `caption`, encabezados, nombres accesibles, región desplazable y scroll interno horizontal/vertical. El diálogo limita ancho y alto al viewport y elimina la clase del `body` al cerrar.
