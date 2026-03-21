# 🐳 Docker Compose – Wichtigste Befehle (n8n-stack)

---

## 🚀 Starten & Stoppen

```bash
# Stack starten (im Hintergrund)
docker compose up -d

# Stack stoppen (Container bleiben erhalten)
docker compose stop

# Stack stoppen + Container löschen (Volumes bleiben!)
docker compose down

# ⚠️ Stack stoppen + Container + Volumes löschen (alle Daten weg!)
docker compose down -v

# Einzelnen Service neu starten
docker compose restart n8n
```

---

## 📋 Status & Logs

```bash
# Status aller Container
docker compose ps

# Live-Logs aller Services
docker compose logs -f

# Live-Logs nur eines Services
docker compose logs -f n8n
docker compose logs -f postgres
docker compose logs -f pgvector

# Letzte 100 Zeilen
docker compose logs --tail=100 n8n
```

---

## 🔄 Updates

```bash
# Neueste Images herunterladen
docker compose pull

# Container mit neuen Images neu erstellen
docker compose up -d --force-recreate
```

---

## 🗄️ Volumes

```bash
# Alle Volumes anzeigen
docker volume ls

# Backup erstellen
docker run --rm \
  -v n8n-stack_n8n_data:/data \
  -v $(pwd):/backup \
  alpine tar czf /backup/n8n_backup.tar.gz -C /data .
```

---

## 🐚 In Container einloggen

```bash
# Shell in n8n
docker exec -it n8n sh

# Shell in postgres
docker exec -it n8n_postgres_db bash

# psql direkt öffnen
docker exec -it n8n_postgres_db psql -U admin -d appdb
```
