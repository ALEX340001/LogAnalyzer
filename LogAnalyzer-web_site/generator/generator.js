const generateBtn = document.getElementById('generate-cli');
const cliFeedback = document.getElementById('cli-feedback');
const output = document.getElementById('commandOutput');

function buildCommand() {
    let parts = ['LogAnalyzer.exe'];

    const inputPath = document.getElementById('inputPath').value.trim();
    if (inputPath) parts.push(`-p "${inputPath}"`);

    const keywords = document.getElementById('keywordsInput').value.trim();
    if (keywords) parts.push(`-k "${keywords}"`);

    const configPath = document.getElementById('configPath').value.trim();
    if (configPath) parts.push(`-c "${configPath}"`);

    const outputPath = document.getElementById('outputPath').value.trim();
    if (outputPath) parts.push(`-o "${outputPath}"`);

    const format = document.getElementById('format').value;
    if (format && format !== 'txt') parts.push(`-f ${format}`);

    // Флаги
    if (document.getElementById('flagAllLines').checked) parts.push('-a');
    if (document.getElementById('flagFinalReport').checked) parts.push('-r');
    if (document.getElementById('flagShowConsole').checked) parts.push('-s false');
    if (document.getElementById('flagDisableLogging').checked) parts.push('-d');

    return parts.join(' ');
}

function copyToClipboard(text, feedbackElement) {
    navigator.clipboard.writeText(text).then(() => {
        feedbackElement.classList.add('show');
        setTimeout(() => feedbackElement.classList.remove('show'), 2000);
    }).catch(() => {
        const textarea = document.createElement('textarea');
        textarea.value = text;
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand('copy');
        document.body.removeChild(textarea);
        feedbackElement.classList.add('show');
        setTimeout(() => feedbackElement.classList.remove('show'), 2000);
    });
}

generateBtn.addEventListener('click', () => {
    const cmd = buildCommand();
    output.value = cmd;
    copyToClipboard(cmd, cliFeedback);
});