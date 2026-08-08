// Загружаем навигацию и футер напрямую (без fetch)
// Работает локально и на сервере

const path = window.location.pathname;
const basePath = path.includes('/generator/') ? '../' : './';

const navHtml = `
<a href="${basePath}index.html">Начало</a>
<div class="dropdown">
    <a href="#" class="dropbtn">Руководство ▾</a>
    <div class="dropdown-content">
        <a href="${basePath}configuration.html">Конфигурация (JSON)</a>
        <a href="${basePath}cli.html">CLI и команды</a>
    </div>
</div>
<div class="dropdown">
    <a href="#" class="dropbtn">Разработка ▾</a>
    <div class="dropdown-content">
        <a href="${basePath}developer.html">Архитектура</a>
        <a href="${basePath}logging.html">Логирование</a>
        <a href="${basePath}dependencies.html">Зависимости</a>
        <a href="${basePath}generator/index.html">Генератор конфигурации</a>
    </div>
</div>
<div class="dropdown">
    <a href="#" class="dropbtn">О проекте ▾</a>
    <div class="dropdown-content">
        <a href="${basePath}license.html">Лицензия</a>
    </div>
</div>
`;

const footerHtml = `
<footer>
    LogAnalyzer · Консольный анализатор логов<br>
    Документация актуальна на 08.2026
</footer>
`;

document.addEventListener('DOMContentLoaded', function() {
    document.getElementById('nav-placeholder').innerHTML = navHtml;
    document.getElementById('footer-placeholder').innerHTML = footerHtml;

    // Обработчик гамбургера для мобильных
    const nav = document.getElementById('nav-placeholder');
    if (nav && !nav.querySelector('.menu-toggle')) {
        const toggleBtn = document.createElement('a');
        toggleBtn.className = 'menu-toggle';
        toggleBtn.innerHTML = '☰';
        toggleBtn.href = '#';
        toggleBtn.addEventListener('click', function(e) {
            e.preventDefault();
            nav.classList.toggle('responsive');
        });
        nav.prepend(toggleBtn);
    }
});
