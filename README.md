# com.ultracombos.template

Unity Template Package

[Unity Package Reference](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest)

## 新增 package 修改步驟

> 進 Unity 前 `（[package-name] 請使用全小寫命名）`

- [ ] 複製 `/com.ultracombos.[package-name]` 整個資料夾，並更改資料夾名稱

- [ ] 編輯 `README.md` 內容

- [ ] 編輯 `package.json` 內容

> 進 Unity 後 `（[PackageName] 請使用大駝峰式命名）`

- [ ] 更改 `/Runtime/UltraCombos.[PackageName].Runtime` 檔名及 **Inspector Name**

- [ ] 更改 `/Editor/UltraCombos.[PackageName].Editor` 檔名及 **Inspector Name**

- [ ] 確認 `/Editor/UltraCombos.[PackageName].Editor` 的 **Assembly Definition References** 內容正確
