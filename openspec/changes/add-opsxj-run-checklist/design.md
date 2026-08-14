## Context

Los comandos OPSXJ ya validan gobierno, evidencia, Git, OpenSpec, GitHub y Jira de manera independiente. `opsxj:status` sintetiza observaciones en tiempo real, mientras que la confirmación de revisión existe únicamente durante el proceso que recibe `OPSXJ_OPENSPEC_REVIEW_CONFIRMED`. Véanse `proposal.md` y la delta de `legacy-opsxj-governance` para el comportamiento requerido.

## Goals / Non-Goals

**Goals:**

- Persistir por ticket y SHA el resultado de las etapas OPSXJ sin secretos ni efectos sobre WebForms.
- Convertir el estado actual en un checklist estable, legible y parseable que explique la siguiente acción.
- Mantener la lectura de tickets históricos y la compatibilidad de `OPSXJ_OPENSPEC_REVIEW_CONFIRMED`.
- Registrar tanto resultados correctos como fallidos sin ocultar ni sobrescribir la historia anterior.

**Non-Goals:**

- No crear una base de datos, servicio web ni interfaz visual.
- No sustituir las comprobaciones vivas de GitHub, Jira, Git u OpenSpec por información almacenada.
- No guardar tokens, variables de entorno completas, datos de sesión, credenciales ni cuerpos de respuestas remotas.
- No alterar la semántica existente de `new`, `refine`, `validate`, `archive` o `close` ni automatizar un merge.

## Decisions

### Registro local versionado y append-only

Se añadirá `tools/opsxj/scripts/lib/runChecklistService.js`. El servicio administrará `.opsxj/runs/<ISSUE-KEY>.json`, que se añadirá a `.gitignore` igual que la evidencia por SHA. Tendrá el formato versionado:

```json
{
  "version": 1,
  "issueKey": "DOC-9",
  "events": [
    {
      "stage": "review",
      "status": "pass",
      "sha": "...",
      "recordedAtUtc": "...",
      "actor": "Codex",
      "source": "opsxj:validate",
      "reference": "formal-review"
    }
  ]
}
```

Los únicos valores aceptados para `stage` serán `new`, `refine`, `review`, `validate`, `archive` y `close`; `status` será `pass` o `fail`. `detail` será opcional, acotado y libre de secretos. Las escrituras usarán archivo temporal y `rename` para evitar JSON truncado. Los eventos nunca se deduplican: la evaluación selecciona la última entrada aplicable para la etapa y SHA, preservando diagnósticos previos.

Alternativas descartadas:

- Reemplazar el registro por un archivo committeado: ensuciaría cada ejecución y haría que datos operativos locales entren al PR.
- Usar solo `.opsxj/evidence`: su modelo reemplaza por tipo y no representa etapas ni errores históricos.
- Usar SQLite: añade una dependencia y complejidad injustificada para un log pequeño local.

### Semántica por SHA y confirmación de revisión

`review` y `validate` serán sensibles al SHA: solo un evento `pass` para el SHA actual satisface la etapa. Si hay un `pass` para otro SHA, el estado calculado será `STALE`; no autoriza `archive`. Cuando `opsxj:validate` recibe `OPSXJ_OPENSPEC_REVIEW_CONFIRMED`, persistirá primero la revisión con `OPSXJ_OPENSPEC_REVIEWED_BY` opcional y luego registrará el resultado de la validación. Sin variable, la validación consultará el registro actual.

La variable temporal seguirá siendo válida para automatizaciones existentes. `opsxj:status` no escribirá archivos: puede mostrar la variable como una observación temporal, pero solo un evento persistido será durable en la siguiente ejecución.

Alternativa descartada: aceptar cualquier revisión histórica. Invalidaría la compuerta después de cambios de código o documentación.

### Integración sin alterar comandos ni efectos remotos

El runner registrará eventos únicamente después de conocer el resultado real de cada comando:

- `new`: al crear con éxito el cambio y su rama.
- `refine`: al terminar su auditoría o sincronización.
- `validate`: siempre, con el conjunto de checks y el SHA evaluado; registra `review` cuando la variable lo confirme.
- `archive`: registra éxito tras archivo, commit/push y PR/comentario Jira; ante una excepción registra fallo antes de propagarla.
- `close`: registra éxito tras validar el PR y cambiar Jira; ante error conserva el fallo.

Los comandos seguirán devolviendo sus códigos de salida actuales. Si no se puede escribir el registro, un comando mutante fallará antes de anunciar éxito, para no comunicar un ciclo incompleto; `status` en cambio mostrará `UNAVAILABLE` y continuará con observaciones vivas.

### Checklist consolidado en estado texto y JSON

`opsxjStatusService` producirá una propiedad `checklist` ordenada: `new`, `refine`, `review`, `validate`, `archive`, `pull_request`, `close`. Cada ítem tendrá `id`, `label`, `state`, `recordedAtUtc`, `sha`, `reference`, `detail` y `nextAction` cuando aplique.

Estados posibles: `COMPLETE`, `PENDING`, `BLOCKED`, `STALE`, `UNAVAILABLE` y `NOT_APPLICABLE`. Las etapas `pull_request` y `close` se calculan principalmente con la consulta viva a GitHub y Jira; `archive` se respalda con el registro y el lifecycle OpenSpec. La salida de texto imprimirá el checklist antes de los indicadores observables existentes; JSON conservará `checks` y agregará `checklist`, para evitar romper consumidores actuales.

Para cambios históricos sin archivo, se devolverá el checklist con `UNAVAILABLE` solo en lo que no pueda inferirse, sin convertir `status` en un fallo ni exigir una migración. La siguiente acción se calculará desde la primera etapa bloqueada, desactualizada o pendiente.

## Risks / Trade-offs

- [Registro local eliminado o corrupto] → Lectura tolerante: `status` conserva chequeos vivos y reporta `UNAVAILABLE`; un comando mutante no reemplaza un archivo inválido sin emitir error explícito.
- [SHA cambia tras validar] → La etapa correspondiente pasa a `STALE` y `archive` sigue exigiendo una validación compatible con el SHA actual.
- [Fallo remoto después de un efecto parcial] → Se registra el error disponible; la siguiente ejecución vuelve a consultar PR/Jira vivos, que son la fuente de verdad remota.
- [Crecimiento del log] → El primer alcance conserva eventos; una política de compactación solo se añadirá si aparece un caso real de tamaño significativo.

## Migration Plan

1. Agregar el servicio, tipos y pruebas unitarias con archivos temporales.
2. Integrarlo en runner, validación y servicio de estado sin eliminar los campos actuales.
3. Añadir `.opsxj/runs/` a `.gitignore` y documentar el formato y la recuperación.
4. Verificar tickets nuevos, uno con SHA desactualizado y uno histórico sin registro.
5. Rollback: retirar las llamadas al servicio; los archivos locales ignorados no afectan compilación, Git, OpenSpec, Jira ni GitHub.
