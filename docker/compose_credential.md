# 🔐 Verbindungsdaten – n8n-stack

> **Wichtig:** Diese Datei enthält sensible Zugangsdaten. Nicht in ein öffentliches Repository einchecken!

---

## 🐘 PostgreSQL (n8n Hauptdatenbank)

| Parameter        | Intern (Docker)  | Extern (Mac/Tools) |
|------------------|------------------|--------------------|
| **Host**         | `postgres`       | `localhost`        |
| **Port**         | `5432`           | `5442`             |
| **Database**     | `appdb`          | `appdb`            |
| **User**         | `admin`          | `admin`            |
| **Password**     | `docker4711%Postgres` | `docker4711%Postgres` |
| **Container**    | `n8n_postgres_db` | –                 |

---

## 🧠 pgvector (Vektor-Datenbank für AI-Workflows)

| Parameter        | Intern (Docker)  | Extern (Mac/Tools) |
|------------------|------------------|--------------------|
| **Host**         | `pgvector`       | `localhost`        |
| **Port**         | `5432`           | `5443`             |
| **Database**     | `appdb`          | `appdb`            |
| **User**         | `admin`          | `admin`            |
| **Password**     | `docker4711%PgVector` | `docker4711%PgVector` |
| **Container**    | `n8n_pgvector_db` | –                 |

---

## ⚙️ n8n

| Parameter        | Wert                        |
|------------------|-----------------------------|
| **URL**          | http://localhost:5678        |
| **Container**    | `n8n`                       |
| **Timezone**     | `Europe/Vienna`             |

---

## 📌 Hinweise

- **Intern** = Verbindung innerhalb des Docker-Netzwerks (z.B. n8n → postgres)
- **Extern** = Verbindung von deinem Mac aus (z.B. TablePlus, DBeaver, psql)
- In **n8n Credentials** immer den internen Host und Port `5432` verwenden
- Für **pgvector** in n8n den Credential-Typ „PostgreSQL" wählen

---

## 🛠️ n8n Credential Einstellungen

### PostgreSQL Credential
```
Type:     PostgreSQL
Host:     postgres
Port:     5432
Database: appdb
User:     admin
Password: docker4711%Postgres
SSL:      disabled
```

### pgvector Credential
```
Type:     PostgreSQL
Host:     pgvector
Port:     5432
Database: appdb
User:     admin
Password: docker4711%PgVector
SSL:      disabled
```
