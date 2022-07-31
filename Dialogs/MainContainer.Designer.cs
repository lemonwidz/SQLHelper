namespace SQLHelper.Dialogs
{
    partial class MainContainer
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnNewTab = new System.Windows.Forms.Button();
            this.btnCloseTab = new System.Windows.Forms.Button();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.btnQueryEditor = new System.Windows.Forms.Button();
            this.cmbDBList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnNewTab
            // 
            this.btnNewTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewTab.Location = new System.Drawing.Point(380, 10);
            this.btnNewTab.Name = "btnNewTab";
            this.btnNewTab.Size = new System.Drawing.Size(100, 23);
            this.btnNewTab.TabIndex = 0;
            this.btnNewTab.Text = "새 탭(테이블)";
            this.btnNewTab.UseVisualStyleBackColor = true;
            this.btnNewTab.Click += new System.EventHandler(this.btnNewTab_Click);
            // 
            // btnCloseTab
            // 
            this.btnCloseTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseTab.Location = new System.Drawing.Point(592, 10);
            this.btnCloseTab.Name = "btnCloseTab";
            this.btnCloseTab.Size = new System.Drawing.Size(100, 23);
            this.btnCloseTab.TabIndex = 1;
            this.btnCloseTab.Text = "현재탭 닫기";
            this.btnCloseTab.UseVisualStyleBackColor = true;
            this.btnCloseTab.Click += new System.EventHandler(this.btnCloseTab_Click);
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Location = new System.Drawing.Point(12, 39);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(680, 630);
            this.tabMain.TabIndex = 2;
            // 
            // btnQueryEditor
            // 
            this.btnQueryEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQueryEditor.Location = new System.Drawing.Point(486, 10);
            this.btnQueryEditor.Name = "btnQueryEditor";
            this.btnQueryEditor.Size = new System.Drawing.Size(100, 23);
            this.btnQueryEditor.TabIndex = 5;
            this.btnQueryEditor.Text = "새 탭(쿼리)";
            this.btnQueryEditor.UseVisualStyleBackColor = true;
            this.btnQueryEditor.Click += new System.EventHandler(this.btnQueryEditor_Click);
            // 
            // cmbDBList
            // 
            this.cmbDBList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDBList.DropDownWidth = 200;
            this.cmbDBList.FormattingEnabled = true;
            this.cmbDBList.Location = new System.Drawing.Point(12, 12);
            this.cmbDBList.Name = "cmbDBList";
            this.cmbDBList.Size = new System.Drawing.Size(178, 20);
            this.cmbDBList.TabIndex = 6;
            // 
            // MainContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 681);
            this.Controls.Add(this.cmbDBList);
            this.Controls.Add(this.btnQueryEditor);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.btnCloseTab);
            this.Controls.Add(this.btnNewTab);
            this.Name = "MainContainer";
            this.Text = "SQL Helper";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnNewTab;
        private System.Windows.Forms.Button btnCloseTab;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.Button btnQueryEditor;
        private System.Windows.Forms.ComboBox cmbDBList;
    }
}

