using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace SPGame.Editor
{
    /// <summary>
    /// Editor 资源路径下拉框工厂
    /// </summary>
    public static class AssetPathDropdownFactory
    {
        public static DropdownField CreateWithFileNameLabels(
            List<string> assetPaths,
            int defaultIndex)
        {
            var dropdownField = new DropdownField(assetPaths, defaultIndex)
            {
                formatListItemCallback = Path.GetFileNameWithoutExtension,
                formatSelectedValueCallback = Path.GetFileNameWithoutExtension
            };

            return dropdownField;
        }
    }
}
