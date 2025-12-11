# Инструкция по настройке CI/CD деплоя

## Настройка SSH на сервере

### Шаг 1: Создание SSH ключа для деплоя

На сервере выполните:

```bash
# Создать SSH ключ без пароля
ssh-keygen -t ed25519 -C "github-actions-deploy" -N "" -f ~/.ssh/github_actions_deploy

# Показать публичный ключ (нужен для следующего шага)
cat ~/.ssh/github_actions_deploy.pub
```

### Шаг 2: Добавление публичного ключа в authorized_keys

```bash
# Добавить публичный ключ в authorized_keys
cat ~/.ssh/github_actions_deploy.pub >> ~/.ssh/authorized_keys

# Установить правильные права доступа (критически важно!)
chmod 700 ~/.ssh
chmod 600 ~/.ssh/authorized_keys
chmod 600 ~/.ssh/github_actions_deploy
```

### Шаг 3: Проверка прав доступа

Убедитесь, что владелец файлов - правильный пользователь:

```bash
# Проверить текущего пользователя
whoami

# Проверить права доступа
ls -la ~/.ssh/

# Должно быть примерно так:
# drwx------ 2 ваш_пользователь ваш_пользователь 4096 ... .ssh
# -rw------- 1 ваш_пользователь ваш_пользователь   xxx ... authorized_keys
# -rw------- 1 ваш_пользователь ваш_пользователь   xxx ... github_actions_deploy
```

### Шаг 4: Получение приватного ключа для GitHub Secrets

```bash
# Показать приватный ключ (скопируйте весь вывод, включая BEGIN и END)
cat ~/.ssh/github_actions_deploy
```

**Важно:** Скопируйте весь ключ целиком, включая строки:
```
-----BEGIN OPENSSH PRIVATE KEY-----
...
-----END OPENSSH PRIVATE KEY-----
```

## Настройка GitHub Secrets

В репозитории GitHub перейдите в **Settings → Secrets and variables → Actions** и добавьте:

1. **`DEPLOY_SSH_KEY`** - приватный ключ из шага 4 (весь ключ целиком)
2. **`DEPLOY_HOST`** - IP адрес или домен вашего сервера (например: `192.168.1.100` или `server.example.com`)
3. **`DEPLOY_USER`** - имя пользователя на сервере (то, что показывает `whoami`)
4. **`DEPLOY_PATH`** - путь к проекту на сервере (например: `/opt/setrep` или `/home/user/setrep`)

## Настройка Git на сервере

Убедитесь, что на сервере настроен Git и репозиторий клонирован:

```bash
# Перейти в директорию проекта
cd /путь/к/проекту

# Проверить, что это git репозиторий
git status

# Если репозиторий не настроен, клонируйте его:
# git clone https://github.com/ваш_username/SetRep.git .
# или
# git clone git@github.com:ваш_username/SetRep.git .
```

## Настройка доступа к GitHub из сервера

Если репозиторий приватный, нужно настроить доступ:

### Вариант 1: Deploy Key (рекомендуется для приватных репозиториев)

1. На сервере создайте отдельный ключ для GitHub:
```bash
ssh-keygen -t ed25519 -C "github-deploy-key" -N "" -f ~/.ssh/github_deploy_key
cat ~/.ssh/github_deploy_key.pub
```

2. В GitHub: **Settings → Deploy keys → Add deploy key**
   - Title: `Production Deploy`
   - Key: вставьте содержимое `~/.ssh/github_deploy_key.pub`
   - ✅ Allow write access (если нужно)

3. Настройте Git для использования этого ключа:
```bash
# Добавить в ~/.ssh/config
cat >> ~/.ssh/config << 'EOF'
Host github.com
    HostName github.com
    User git
    IdentityFile ~/.ssh/github_deploy_key
    IdentitiesOnly yes
EOF

chmod 600 ~/.ssh/config
```

### Вариант 2: Personal Access Token (для HTTPS)

Если используете HTTPS для клонирования:

```bash
# Настроить Git для использования токена
git remote set-url origin https://ваш_токен@github.com/ваш_username/SetRep.git
```

## Тестирование SSH подключения

Перед настройкой GitHub Actions, протестируйте подключение вручную:

```bash
# С локального компьютера или с самого сервера
ssh -i ~/.ssh/github_actions_deploy ваш_пользователь@ваш_сервер "echo 'SSH работает!'"
```

Если команда выполняется без запроса пароля - всё настроено правильно!

## Проверка после настройки

После настройки всех секретов в GitHub:

1. Сделайте коммит в ветку `production`
2. Проверьте выполнение workflow в **Actions** вкладке
3. Если есть ошибки, проверьте логи в Actions

## Устранение проблем

### Ошибка "Permission denied (publickey)"

1. Проверьте, что публичный ключ добавлен в `~/.ssh/authorized_keys`
2. Проверьте права доступа: `chmod 700 ~/.ssh` и `chmod 600 ~/.ssh/authorized_keys`
3. Проверьте, что пользователь в секрете `DEPLOY_USER` правильный
4. Проверьте логи SSH на сервере: `sudo tail -f /var/log/auth.log` (Linux) или `sudo tail -f /var/log/system.log` (Mac)

### Ошибка "Host key verification failed"

Добавьте сервер в known_hosts в workflow (уже добавлено автоматически через `ssh-keyscan`)

### Ошибка "git: command not found"

Установите Git на сервере:
```bash
# Ubuntu/Debian
sudo apt-get update && sudo apt-get install -y git

# CentOS/RHEL
sudo yum install -y git
```

### Ошибка "docker-compose: command not found"

Убедитесь, что Docker и Docker Compose установлены на сервере. Workflow автоматически определяет версию (V1 или V2).

