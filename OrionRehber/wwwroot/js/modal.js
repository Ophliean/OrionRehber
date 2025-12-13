// ===================== ORTAK YARDIMCI FONKSİYONLAR =====================

// CSRF token'ını al
function getRequestVerificationToken() {
    const tokenInput = document.querySelector("input[name='__RequestVerificationToken']");
    return tokenInput ? tokenInput.value : "";
}

// Toast mesajı göster
function showMessage(text, type = "success") {
    const div = document.createElement("div");
    div.className = "toast " + type;
    div.innerText = text;

    document.body.appendChild(div);

    // animasyon için hafif gecikme
    setTimeout(() => div.classList.add("show"), 10);
    setTimeout(() => div.classList.remove("show"), 2500);
    setTimeout(() => div.remove(), 3000);
}

// ===================== ADD / EDIT MODALI =====================

// Modalı açar (Yeni Kayıt)
function openAddModal() {
    document.getElementById("modalTitle").innerText = "Yeni Kayıt";

    document.getElementById("Id").value = "";
    document.getElementById("Ad").value = "";
    document.getElementById("Soyad").value = "";
    document.getElementById("Telefon").value = "";

    document.getElementById("addEditModal").classList.remove("hidden");
}

// Modalı açar (Düzenleme)
function openEditModal(id) {
    fetch(`/Rehber/Get/${id}`)
        .then(res => res.json())
        .then(data => {
            document.getElementById("modalTitle").innerText = "Kayıt Düzenle";

            document.getElementById("Id").value = data.id;
            document.getElementById("Ad").value = data.ad;
            document.getElementById("Soyad").value = data.soyad;
            document.getElementById("Telefon").value = data.telefon;

            document.getElementById("addEditModal").classList.remove("hidden");
        })
        .catch(err => {
            console.error("Kayıt getirme hatası:", err);
            showMessage("Kayıt getirilirken hata oluştu.", "error");
        });
}

// Modalı kapatır
function closeAddEditModal() {
    document.getElementById("addEditModal").classList.add("hidden");
}

// ===================== SİLME MODALI =====================

// Silinecek id'yi burada tutacağız
let deleteId = null;

// Tablodaki 🗑 butonu bunu çağırıyor
function deleteRecord(id) {
    deleteId = id;

    const modal = document.getElementById("deleteModal");
    if (modal) {
        modal.classList.remove("hidden");
    } else {
        console.error("deleteModal bulunamadı. _AddEditModal partial'ı sayfaya ekli mi?");
        showMessage("Silme ekranı bulunamadı.", "error");
    }
}

// Silme modalını kapat
function closeDeleteModal() {
    deleteId = null;

    const modal = document.getElementById("deleteModal");
    if (modal) {
        modal.classList.add("hidden");
    }
}

// ===================== EVENT BAĞLAMA =====================

document.addEventListener("DOMContentLoaded", function () {

    // ADD / EDIT FORM SUBMIT — ADD / EDIT AYIRT EDER
    const form = document.getElementById("addEditForm");
    if (form) {
        form.addEventListener("submit", function (e) {
            e.preventDefault();

            const formData = new FormData(form);
            const token = getRequestVerificationToken();

            // Add / Edit seçimi
            const id = document.getElementById("Id").value;
            const url = id === "" ? "/Rehber/Add" : "/Rehber/Edit";

            fetch(url, {
                method: "POST",
                headers: {
                    "RequestVerificationToken": token
                },
                body: formData
            })
                .then(res => res.json())
                .then(data => {
                    if (!data.success) {
                        showMessage(data.message || "İşlem sırasında hata oluştu.", "error");
                        return;
                    }

                    showMessage(data.message || "İşlem başarılı.", "success");

                    // Mesajı görsün diye ufak gecikme
                    setTimeout(() => location.reload(), 1000);
                })
                .catch(err => {
                    console.error("Beklenmeyen hata:", err);
                    showMessage("Beklenmeyen bir hata oluştu!", "error");
                });
        });
    }

    // SİLME ONAY BUTONU
    const confirmDeleteBtn = document.getElementById("confirmDeleteBtn");
    if (confirmDeleteBtn) {
        confirmDeleteBtn.addEventListener("click", function () {

            if (deleteId == null) {
                closeDeleteModal();
                return;
            }

            const token = getRequestVerificationToken();

            fetch("/Rehber/Delete", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded;charset=UTF-8",
                    "RequestVerificationToken": token
                },
                body: "id=" + encodeURIComponent(deleteId)
            })
                .then(res => res.json())
                .then(data => {
                    if (!data.success) {
                        showMessage(data.message || "Silme işlemi başarısız.", "error");
                        return;
                    }

                    showMessage(data.message || "Kayıt silindi.", "success");

                    setTimeout(() => location.reload(), 1000);
                })
                .catch(err => {
                    console.error("Silme hatası:", err);
                    showMessage("Silme sırasında beklenmeyen bir hata oluştu.", "error");
                });

            closeDeleteModal();
        });
    }
});
