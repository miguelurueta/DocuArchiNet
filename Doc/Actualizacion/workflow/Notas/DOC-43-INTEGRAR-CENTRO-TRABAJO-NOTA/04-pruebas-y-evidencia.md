# Pruebas y evidencia

| Control | Resultado |
|---|---|
| `node --check js/workflow/Webworkflow.js` | PASS |
| `node --test tools/e2e/tests/doc43-notes-ui-policy.test.cjs` | 8/8 PASS |
| `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m` | PASS, 0 errores; advertencias históricas |
| E2E `test:doc43:notes-ui` | 1/1 PASS en 20.1 s; contenido especial incluido |
| OpenSpec strict | PASS |

QA visual Chromium: 375×812, 768×1024, 1024×768, 1440×900 y 812×375. Evidencia saneada bajo `.opsxj/evidence`; no contiene secretos. La E2E comprobó crear, mostrar, editar y eliminar contenido con Unicode, comillas, salto de línea y cadena tipo XSS sobre una tarea descartable autorizada. Al terminar: gate `false`, usuarios vacíos y grupos vacíos.

Limitación: la corrida autenticada cubrió Chromium en el ambiente autorizado; otros navegadores/dispositivos no estuvieron disponibles.
