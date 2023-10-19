using UnityEngine;
using System.Collections;

namespace EnhancedUI.EnhancedScroller {
    
    /// <summary>
    ///     Represents a UIElement used to be visualized in a list.
    ///     Contains several methods for assigning indexes and identifier to the list entrys.
    ///     Replaces "EnchancedScrollCellView.cs from the EnchancedScroller Plugin/Asset"
    /// </summary>
    public class ListElement : MonoBehaviour {


        ///////////////////////
        /// Main methods
        ///////////////////////

        /// <summary>
        ///     This method is called by the scroller when the RefreshActiveCellViews is called on the scroller
        ///     You can override it to update your cell's view UID
        /// </summary>
        public virtual void RefreshCellView() { }

        /// <summary>
        ///     The cellIdentifier is a unique string that allows the scroller
        ///     to handle different types of cells in a single list. Each type
        ///     of cell should have its own identifier
        /// </summary>
        public string CellIdentifier { get; set; }

        /// <summary>
        ///     The cell index of the cell view
        ///     This will differ from the dataIndex if the list is looping
        /// </summary>
        public int CellIndex { get; set; }

        /// <summary>
        ///     The data index of the cell view
        /// </summary>
        public int DataIndex { get; set; }

        /// <summary>
        ///     Whether the cell is active or recycled
        /// </summary>
        public bool Active { get; set; }
    }
    
    /// <summary>
    /// All scripts that handle the scroller's callbacks should inherit from this interface
    /// </summary>
    public interface IEnhancedScrollerDelegate{
        
        /// <summary>
        /// Gets the number of cells in a list of data
        /// </summary>
        /// <param name="scroller"></param>
        /// <returns></returns>
        int GetNumberOfCells(EnhancedScroller scroller);

        /// <summary>
        /// Gets the size of a cell view given the index of the data set.
        /// This allows you to have different sized cells
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        float GetCellViewSize(EnhancedScroller scroller, int dataIndex);

        /// <summary>
        /// Gets the cell view that should be used for the data index. Your implementation
        /// of this function should request a new cell from the scroller so that it can
        /// properly recycle old cells.
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        ListElement GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex);
    }
}